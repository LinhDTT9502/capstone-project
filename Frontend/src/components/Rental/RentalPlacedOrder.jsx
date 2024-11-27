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
        cartItemId: product.id,  
        productId: product.productId,
        productCode: product.productCode,
        productName: product.productName,
        quantity: product.quantity,
        size: product.size,
        color: product.color,
        condition: product.condition,
        rentPrice: product.rentPrice,
        imgAvatarPath: product.imgAvatarPath,
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

  return (<>



    <div className="flex flex-row bg-slate-200">
      <div className="text-nowrap basis-2/3 bg-white">
        <OrderMethod
          userData={userData}
          setUserData={setUserData}
          selectedOption={selectedOption}
          handleOptionChange={handleOptionChange}
          selectedBranchId={branchId}
          setSelectedBranchId={setBranchId}
        />
      </div>
      <div className="basis-3/5  pr-20 pl-5 h-1/4 mt-10">
        <div className="font-alfa text-center p-5 border rounded text-black">
          Summary
        </div>
        <div className="overflow-auto h-3/4">
          <div className="grid grid-cols-1 gap-4">
            {selectedProducts.map((product) => (
              <div key={product.id} className="flex border rounded p-4 space-x-2">
                <div className="relative">
                  <img
                    src={product.imgAvatarPath}
                    alt={product.productName}
                    className="w-auto h-32 object-scale-down rounded"
                  />
                  <span className="absolute top-0 right-0 bg-orange-500 text-white text-xs font-bold rounded-full w-5 h-5 flex items-center justify-center">
                    {product.quantity}
                  </span>
                </div>
                <div className="flex justify-between w-full">
                  <div className="flex flex-col space-y-4">
                    <h3 className="text-lg font-semibold">
                      {product.productName}
                    </h3>
                    <div className="text-sm">
                      <li>Màu sắc: {product.color}</li>
                      <li>Kích cỡ: {product.size}</li>
                      <li>Tình trạng: {product.condition}%</li>
                    </div>
                  </div>
                  <p className="text-lg text-black">{(product.price * product.quantity).toLocaleString()} VND</p>
                </div>
              </div>
            ))}
          </div>
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

          <div className="h-px bg-gray-300 my-5 mx-auto font-bold"></div>
          <div className="flex justify-between items-center pt-1 border rounded mt-4">
            <h3 className="text-lg font-semibold">
              Tạm tính
            </h3>
            <p className="text-lg text-black">
              1 VND
            </p>
          </div>
          <div className="flex justify-between items-center pt-1 border rounded mt-4">
            <label className="block text-lg font-semibold">Ghi chú</label>
            <input
              type="text"
              className="border rounded w-3/4 px-3 py-2 mt-2"
              value={note}
              // onChange={(e) => setNote(e.target.value)}
              placeholder="Ghi chú của bạn"
            />
          </div>
          <div className="flex justify-between items-center pt-1 border rounded mt-4">
            <h3 className="text-lg font-semibold">
              Phí vận chuyển
            </h3>
            <p className="text-lg text-black">
              2Sport sẽ liên hệ và thông báo sau
            </p>
          </div>
          <div className="flex justify-between items-center pt-1 border rounded mt-4">
            <h3 className="text-lg font-semibold">Tổng cộng</h3>
            <p className="text-lg text-black">
              2 VND
            </p>
          </div>
        </div>
        <div className="flex pb-10 justify-center items-center">
          <button
            onClick={handleCreateRentalOrder}
            disabled={loading}
            className="bg-orange-500 text-white px-4 py-2 rounded-md"
          >
            {loading ? "Processing..." : "Create Rental Order"}
          </button>
        </div>
      </div>
    </div>
  </>
  );
};

export default RentalPlacedOrder;