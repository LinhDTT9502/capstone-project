import { useState, useEffect } from "react";
import axios from "axios";
import { useNavigate, useLocation } from "react-router-dom";
import { useSelector } from "react-redux";
import { selectUser } from "../../redux/slices/authSlice";
import OrderMethod from "../Order/OrderMethod";

const RentalPlacedOrder = () => {
  const user = useSelector(selectUser);
  const navigate = useNavigate();
  const location = useLocation();
  const { selectedProducts } = location.state || {}; // Get selectedProducts from state
  const [branchId, setBranchId] = useState(null);
  const [selectedOption, setSelectedOption] = useState("");
  const [discountCode, setDiscountCode] = useState("");
  const [userData, setUserData] = useState({
    fullName: "",
    gender: "",
    email: "",
    phoneNumber: "",
    address: "",
  });
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [note, setNote] = useState("");
  const [loading, setLoading] = useState(false);
  const [apiResponse, setApiResponse] = useState(null);

  const token = localStorage.getItem("token");

  const handleStartDateChange = (e) => setStartDate(e.target.value);
  const handleEndDateChange = (e) => setEndDate(e.target.value);

  const getTomorrowDate = () => {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    return tomorrow.toISOString().split("T")[0];
  };

  const handleCreateRentalOrder = async () => {
    if (!selectedProducts || selectedProducts.length === 0) {
      alert("No product selected for rental.");
      return;
    }

    if (!startDate || !endDate || !selectedOption || !userData.fullName) {
      alert("Please fill in all required fields.");
      return;
    }

    if (new Date(startDate) >= new Date(endDate)) {
      alert("End date must be after the start date.");
      return;
    }

    const payload = {
      customerInformation: {
        userId: token ? user.UserId : 0,
        email: userData.email,
        fullName: userData.fullName,
        gender: userData.gender,
        contactPhone: userData.phoneNumber,
        address: userData.address,
      },
      note: "Test",
      deliveryMethod: selectedOption,
      branchId: selectedOption === "STORE_PICKUP" ? branchId : null,
      productInformations: selectedProducts.map(product => ({
        cartItemId: product.id,  // Use the ID from selectedProducts
        productId: product.productId,
        productCode: product.productCode,
        productName: product.productName,
        quantity: product.quantity,
        rentPrice: 120000, // Adjust according to your data structure
        rentalDates: {
          dateOfReceipt: new Date(startDate).toISOString(),
          rentalStartDate: new Date(startDate).toISOString(),
          rentalEndDate: new Date(endDate).toISOString(),
          rentalDays: (new Date(endDate) - new Date(startDate)) / (1000 * 60 * 60 * 24) + 1,
        },
        rentalCosts: {
          subTotal: 120000 * product.quantity,
          tranSportFee: 0,
          totalAmount: 120000 * product.quantity,
        },
      })),
    };
console.log(payload);

    try {
      setLoading(true);
      const response = await axios.post(
        "https://twosportapi-295683427295.asia-southeast2.run.app/api/RentalOrder/create",
        payload,
        {
          headers: {
            "Authorization": `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );
      setApiResponse(response.data.data);
      navigate("/order_success", {
        state: {
          orderID: response.data.data.id,
          orderCode: response.data.data.rentalOrderCode,
        },
      });
      alert("Rental order created successfully!");
    } catch (error) {
      console.error("Error creating rental order:", error);
      alert("Failed to create rental order.");
    } finally {
      setLoading(false);
    }
  };

  const handleOptionChange = (event) => {
    setSelectedOption(event.target.value);
  };

  if (!selectedProducts || selectedProducts.length === 0) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-slate-200">
        <p className="text-gray-600 text-lg font-medium">
          No product selected for rental.
        </p>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-slate-200 py-10 px-4 md:px-8">
      <div className="max-w-4xl mx-auto bg-white shadow-lg rounded-lg">
        {/* Product Details */}
        <div className="p-6">
          {selectedProducts.map((product) => (
            <div key={product.id}>
              <h2 className="text-xl font-bold text-gray-800">{product.productName}</h2>
              <p>Code: {product.productCode}</p>
              <p>Quantity: {product.quantity}</p>
              <p>Rent Price: {product.rentPrice}</p>
            </div>
          ))}
        </div>

        {/* Date Pickers */}
        <div className="px-6 pb-6">
          <label>Start Date:</label>
          <input
            type="date"
            min={getTomorrowDate()}
            value={startDate}
            onChange={handleStartDateChange}
          />
          <label>End Date:</label>
          <input
            type="date"
            min={startDate || getTomorrowDate()}
            value={endDate}
            onChange={handleEndDateChange}
          />
        </div>

        {/* Order Method */}
        <OrderMethod
          userData={userData}
          setUserData={setUserData}
          selectedOption={selectedOption}
          handleOptionChange={handleOptionChange}
          selectedBranchId={branchId}
          setSelectedBranchId={setBranchId}
        />

        {/* Submit Button */}
        <div className="px-6 pb-6">
          <button
            onClick={handleCreateRentalOrder}
            disabled={loading}
            className="bg-blue-500 text-white px-4 py-2 rounded-md"
          >
            {loading ? "Processing..." : "Create Rental Order"}
          </button>
        </div>
      </div>
    </div>
  );
};

export default RentalPlacedOrder;