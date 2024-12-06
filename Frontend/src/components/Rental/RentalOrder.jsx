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
        userId: token ? user.UserId : null,
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
          size: product.size,
          color: product.color,
          condition: product.condition,
          imgAvatarPath: product.imgAvatarPath,
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

    try {
      setLoading(true);
      const response = await axios.post(
        "https://capstone-project-703387227873.asia-southeast1.run.app/api/RentalOrder/create",
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

  const handleOptionChange = (event) => setSelectedOption(event.target.value);

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
    <div className="min-h-screen flex bg-slate-200">
      <div className="flex-1 bg-white p-6">
        <OrderMethod
          userData={userData}
          setUserData={setUserData}
          selectedOption={selectedOption}
          handleOptionChange={handleOptionChange}
          selectedBranchId={branchId}
          setSelectedBranchId={setBranchId}
        />
      </div>
      <div className="flex-1 bg-slate-200  p-6 overflow-y-auto mt-10">
        <div className="font-alfa bg-white text-center p-5 border rounded text-black mb-5">
          <h2 className="text-xl">Tóm tắt đơn hàng</h2>
        </div>
        <div className="space-y-4 ">
          <div className="flex bg-white items-center space-x-4 border p-4 rounded">
            <div className="relative">
              <img
                src={rentalData.product.imgAvatarPath}
                alt={rentalData.product.productName}
                className="w-32 h-32 object-contain rounded"
              />
              <span className="absolute top-0 right-0 bg-orange-500 text-white text-xs font-bold rounded-full w-5 h-5 flex items-center justify-center">
                {rentalData.quantity}
              </span>
            </div>
            <div className="flex-1">
              <h3 className="text-lg font-semibold">{rentalData.product.productName}</h3>
              <ul className="text-sm">
                <li>Màu sắc: {rentalData.product.color}</li>
                <li>Kích thước: {rentalData.product.size}</li>
                <li>Tình trạng: {rentalData.product.condition}%</li>
              </ul>
            </div>
            <p className="text-lg font-semibold">{(rentalData.product.rentPrice * rentalData.quantity).toLocaleString()} ₫</p>
          </div>
          <div className="space-y-4 px-6 py-4">
            <div>
              <p className="text-xl font-semibold">Chọn thời gian thuê</p>
              <hr className="my-2"/>
              <label className="block text-sm">Ngày bắt đầu:</label>
              <input
                type="date"
                min={getTomorrowDate()}
                value={startDate}
                onChange={handleStartDateChange}
                className="border rounded px-4 py-2 w-full"
              />
            </div>
            <div>
              <label className="block text-sm">Ngày kết thúc:</label>
              <input
                type="date"
                min={startDate || getTomorrowDate()}
                value={endDate}
                onChange={handleEndDateChange}
                className="border rounded px-4 py-2 w-full"
              />
            </div>
            <div>
              <label className="block text-sm">Ghi chú:</label>
              <input
                type="text"
                className="border rounded w-full px-4 py-2 mt-2"
                value={note}
                onChange={(e) => setNote(e.target.value)}
                placeholder="Your notes here"
              />
            </div>
            <div className="h-px bg-gray-300 my-5"></div>
            <div className="flex justify-between items-center">
              <h3 className="text-lg font-semibold">Phí giao hàng</h3>
              <p className="text-lg">Sẽ được báo lại từ 2Sport</p>
            </div>
            <div className="flex justify-between items-center pt-1 mt-4">
              <h3 className="text-lg font-semibold">Tổng giá</h3>
              <p className="text-lg">{(rentalData.product.rentPrice * rentalData.quantity).toLocaleString()} ₫</p>
            </div>
          </div>
          <div className="flex justify-center mt-6">
            <button
              onClick={handleCreateRentalOrder}
              disabled={loading}
              className={`bg-orange-500 text-white w-full py-3 rounded ${loading ? "opacity-50" : ""}`}
            >
              {loading ? "Đang tiến hành..." : "Tạo đơn hàng"}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default RentalOrder;
