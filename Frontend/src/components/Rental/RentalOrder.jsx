import { useState, useEffect } from "react";
import axios from "axios";
import { useNavigate, useLocation } from "react-router-dom";
import { useSelector } from "react-redux";
import { selectUser } from "../../redux/slices/authSlice";
import OrderMethod from "../Order/OrderMethod";
// import ProductType from "../Product/ProductType";

const RentalOrder = () => {
  const user = useSelector(selectUser);
  const navigate = useNavigate();
  const location = useLocation();
  const { selectedProducts } = location.state || { selectedProducts: [] };

  const [branchId, setBranchId] = useState(null);
  const [selectedOption, setSelectedOption] = useState("");
  const [rentalData, setRentalData] = useState(null);
  const [discountCode, setDiscountCode] = useState("");
  const [userData, setUserData] = useState({
    fullName: "",
    gender: "",
    email: "",
    phoneNumber: "",
    address: "",
    shipmentDetailID: 0,
  });
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [note, setNote] = useState("");
  const [loading, setLoading] = useState(false);
  const [apiResponse, setApiResponse] = useState(null);
  console.log(rentalData);

  const token = localStorage.getItem("token");

  useEffect(() => {
    const savedRentalData = JSON.parse(localStorage.getItem("rentalData"));

    if (selectedProducts && selectedProducts.length > 0) {
      setRentalData({
        products: selectedProducts.map((product) => ({
          id: product.id,
          productCode: product.productCode,
          productName: product.productName,
          rentPrice: product.rentPrice,
          quantity: product.quantity || 1, // Default quantity
        })),
      });
     
      
      localStorage.removeItem("rentalData");
    } else if (savedRentalData) {
      setRentalData(savedRentalData);
    }
  }, [selectedProducts]);

  const handleStartDateChange = (e) => setStartDate(e.target.value);
  const handleEndDateChange = (e) => setEndDate(e.target.value);

  const getTomorrowDate = () => {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    return tomorrow.toISOString().split("T")[0];
  };

  const handleCreateRentalOrder = async () => {
    if (!rentalData || !rentalData.products || rentalData.products.length === 0) {
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

    const dateOfReceipt = new Date(startDate);
    dateOfReceipt.setDate(dateOfReceipt.getDate() + 1);

    const payload = {
      fullName: userData.fullName,
      email: userData.email,
      contactPhone: userData.phoneNumber,
      address: userData.address,
      userID: token ? user.UserId : 0,
      shipmentDetailID: userData.shipmentDetailID,
      deliveryMethod: selectedOption,
      gender: userData.gender,
      branchId: selectedOption === "STORE_PICKUP" ? branchId : null,
      discountCode: discountCode || null,
      note: note || null,
      dateOfReceipt: dateOfReceipt.toISOString(),
      rentalOrderItemCMs: rentalData.products.map((product) => ({
        productId: product.id,
        productCode: product.productCode,
        productName: product.productName,
        quantity: product.quantity,
        rentPrice: product.rentPrice,
        rentalStartDate: new Date(startDate).toISOString(),
        rentalEndDate: new Date(endDate).toISOString(),
      })),
    };

    try {
      setLoading(true);
      const response = await axios.post(
        "https://twosportapi-295683427295.asia-southeast2.run.app/api/RentalOrder/create-rental-order",
        payload,
        { headers: { "Content-Type": "application/json" } }
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

  if (!rentalData || !rentalData.products || rentalData.products.length === 0) {
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
        {rentalData.products.map((product, index) => (
          <div className="p-6" key={index}>
            <h2 className="text-xl font-bold text-gray-800">
              {product.productName}
            </h2>
            <p>Code: {product.productCode}</p>
            <p>Quantity: {product.quantity}</p>
            <p>Rent Price: {product.rentPrice}</p>
          </div>
        ))}

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
          handleOptionChange={setSelectedOption}
          selectedBranchId={branchId}
          setSelectedBranchId={setBranchId}
        />

        {/* Submit Button */}
        <div className="px-6 pb-6">
          <button
            onClick={handleCreateRentalOrder}
            disabled={loading}
            className={`w-full py-2 px-4 rounded-md text-white ${
              loading ? "bg-gray-500" : "bg-blue-600 hover:bg-blue-700"
            }`}
          >
            {loading ? "Processing..." : "Create Rental Order"}
          </button>
        </div>
      </div>
    </div>
  );
};

export default RentalOrder;
