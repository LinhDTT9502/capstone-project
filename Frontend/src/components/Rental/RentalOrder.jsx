import { useState, useEffect } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import { useSelector } from "react-redux";
import { selectUser } from "../../redux/slices/authSlice";
import OrderMethod from "../Order/OrderMethod";

const RentalOrder = () => {
  const user = useSelector(selectUser);
  const navigate = useNavigate();
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
  // console.log(selectedOption);
  

  const token = localStorage.getItem("token");

  const rentalData = JSON.parse(localStorage.getItem("rentalData"));

  const handleStartDateChange = (e) => setStartDate(e.target.value);
  const handleEndDateChange = (e) => setEndDate(e.target.value);

  const getTomorrowDate = () => {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    return tomorrow.toISOString().split("T")[0];
  };

  const handleCreateRentalOrder = async () => {
    if (!rentalData || !rentalData.product) {
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

    // Assuming rentalData.product contains a single product object
    const product = rentalData.product;

    const payload = {
      customerInformation: {
        userId: token ? user.UserId : 0,
        email: userData.email,
        fullName: userData.fullName,
        gender: userData.gender,
        contactPhone: userData.phoneNumber,
        address: userData.address,
      },
      note: note || null,
      deliveryMethod: selectedOption,
      branchId: selectedOption === "STORE_PICKUP" ? branchId : null,
      productInformations: [
        {
          cartItemId: null,
          productId: product.id,
          productCode: product.productCode,
          productName: product.productName,
          quantity: rentalData.quantity,
          rentPrice: product.rentPrice,
          rentalDates: {
            dateOfReceipt: new Date(new Date(startDate).setDate(new Date(startDate).getDate() - 1)).toISOString(),
            rentalStartDate: new Date(startDate).toISOString(),
            rentalEndDate: new Date(endDate).toISOString(),
            rentalDays: (new Date(endDate) - new Date(startDate)) / (1000 * 60 * 60 * 24) + 1,  // Dividing by milliseconds in a day
          },          
          rentalCosts: {
            subTotal: product.rentPrice * rentalData.quantity,
            tranSportFee: 0, // Assuming no transport fee; adjust if needed
            totalAmount: product.rentPrice * rentalData.quantity,
          },
        },
      ],
    };
// console.log(payload);

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
    // if (event.target.value === "STORE_PICKUP") {
     
    // }
  };

  if (!rentalData || !rentalData.product) {
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
          <h2 className="text-xl font-bold text-gray-800">{rentalData.product.productName}</h2>
          <p>Code: {rentalData.product.productCode}</p>
          <p>Quantity: {rentalData.quantity}</p>
          <p>Rent Price: {rentalData.product.rentPrice}</p>
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

export default RentalOrder;