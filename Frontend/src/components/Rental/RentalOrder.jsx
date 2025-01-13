import { useState, useEffect } from "react";
import axios from "axios";
import { useNavigate, Link } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { selectUser } from "../../redux/slices/authSlice";
import OrderMethod from "../Order/OrderMethod";
import { toast, ToastContainer } from "react-toastify";
import { addGuestRentalOrder } from "../../redux/slices/guestOrderSlice";
import { faInfoCircle } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Button } from "@material-tailwind/react";
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
  const dispatch = useDispatch();
  const [showTooltip, setShowTooltip] = useState(false);

  const handleStartDateChange = (e) => {
    const newStartDate = e.target.value;
    setStartDate(newStartDate);

    const nextDay = new Date(newStartDate);
    nextDay.setDate(nextDay.getDate() + 1);
    // setEndDate(nextDay.toISOString().split("T")[0]);
  };

  const handleEndDateChange = (e) => setEndDate(e.target.value);

  const getTomorrowDate = () => {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    return tomorrow.toISOString().split("T")[0];
  };

  const calculateRentalDays = () => {
    if (!startDate || !endDate) return 0;
    const start = new Date(startDate);
    const end = new Date(endDate);
    return Math.ceil((end - start) / (1000 * 60 * 60 * 24)) + 1;
  };

  const rentalDays = calculateRentalDays();

  const handleCreateRentalOrder = async () => {
    setLoading(true);
    try {
      if (!selectedOption) {
        toast.error("Vui lòng chọn phương thức giao hàng.");
        return;
      }
      if (selectedOption === "HOME_DELIVERY" && !userData.address.trim()) {
        toast.error("Vui lòng nhập địa chỉ giao hàng.");
        return;
      }

      if (!rentalData || !rentalData.product) {
        toast.error("Không có sản phẩm nào được chọn để thuê.");
        return;
      }
      if (!userData.fullName.trim()) {
        toast.error("Vui lòng nhập họ và tên của bạn.");
        return;
      }
      if (!/^\S+@\S+\.\S+$/.test(userData.email)) {
        toast.error("Vui lòng nhập email hợp lệ.");
        return;
      }
      if (!/^[0-9]{10}$/.test(userData.phoneNumber)) {
        toast.error("Số điện thoại phải có 10 chữ số.");
        return;
      }
      if (!startDate) {
        toast.error("Vui lòng nhập ngày bắt đầu thuê.");
        return;
      }
      if (!endDate) {
        toast.error("Vui lòng nhập ngày kết thúc thuê.");
        return;
      }
      if (new Date(startDate) >= new Date(endDate)) {
        toast.error("Ngày kết thúc phải sau ngày bắt đầu.");
        return;
      }

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
              dateOfReceipt: new Date(
                new Date(startDate).setDate(new Date(startDate).getDate() - 1)
              ).toISOString(),
              rentalStartDate: new Date(startDate).toISOString(),
              rentalEndDate: new Date(endDate).toISOString(),
              rentalDays: rentalDays,
            },
            rentalCosts: {
              subTotal: product.rentPrice * rentalData.quantity * rentalDays,
              tranSportFee: 0,
              totalAmount: product.rentPrice * rentalData.quantity * rentalDays,
            },
          },
        ],
      };

      console.log("Dữ liệu hợp lệ, bắt đầu gửi yêu cầu...");
      const response = await axios.post(
        "https://twosport-api-offcial-685025377967.asia-southeast1.run.app//api/RentalOrder/create",
        payload,
        {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );
      const orderID = response.data.data.id;
      const orderRentalCode = response.data.data.rentalOrderCode
      console.log(response);
      
      if (!token) {
        dispatch(addGuestRentalOrder(response.data.data));
      }
      setApiResponse(response.data.data);
      navigate("/order_success", {
        state: {
          orderID: orderID,
          orderCode: null,
          rentalOrderCode: orderRentalCode
        },
      });
      toast.success("Đơn hàng đã được tạo thành công!");
    } catch (error) {
      setLoading(false);
      console.error("Lỗi tạo đơn hàng:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleOptionChange = (event) => setSelectedOption(event.target.value);

  if (!rentalData || !rentalData.product) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-slate-200">
        <p className="text-gray-600 text-lg font-medium">
          Không có sản phẩm nào được chọn
        </p>
      </div>
    );
  }
const productArray = [rentalData.product];
  return (
    <>
      <ToastContainer position="top-right" autoClose={3000} />
      <div className="min-h-screen flex bg-slate-200">
        <div className="flex-1 bg-white p-6">
          <OrderMethod
            userData={userData}
            setUserData={setUserData}
            selectedOption={selectedOption}
            handleOptionChange={handleOptionChange}
            selectedBranchId={branchId}
            setSelectedBranchId={setBranchId}
            selectedProducts={productArray}
          />
        </div>
        <div className="flex-1 bg-slate-200 p-6 overflow-y-auto mt-10">
          <div className="font-extrabold bg-white text-center p-5 border rounded text-black mb-5">
            <h2 className="text-xl">Thông tin đơn hàng - Đơn thuê</h2>
          </div>
          <div className="space-y-4 ">
            <div className="p-4 rounded bg-gray-50">
              <div className="flex flex-wrap items-start space-x-4 border p-4 rounded bg-white">
                {/* Phần ảnh sản phẩm */}
                <div className="relative">
                  <div className="bg-white">
                    <img
                      src={rentalData.product.imgAvatarPath}
                      alt={rentalData.product.productName}
                      className="w-32 h-32 object-contain rounded"
                    />
                  </div>
                  <span className="absolute top-0 right-0 bg-orange-500 text-white text-xs font-bold rounded-full w-5 h-5 flex items-center justify-center">
                    {rentalData.quantity}
                  </span>
                </div>
                <div className="flex flex-col flex-1">
                  <div className="flex flex-wrap md:flex-nowrap items-start gap-6">
                    {/* Thông tin sản phẩm */}
                    <div className="flex-1 min-w-[200px]">
                      <h3 className="text-lg font-semibold">
                        {rentalData.product.productName}
                      </h3>
                      <ul className="text-sm">
                        <li>Màu sắc: {rentalData.product.color}</li>
                        <li>Kích thước: {rentalData.product.size}</li>
                        <li>Tình trạng: {rentalData.product.condition}%</li>
                        <li className="text-rose-700">
                          Đơn giá thuê:{" "}
                          {rentalData.product.rentPrice.toLocaleString(
                            "Vi-vn"
                          ) || "Đang tính.."}
                          ₫/ngày
                        </li>
                      </ul>
                    </div>
                    {/* Thời gian thuê */}
                    <div className="flex flex-col space-y-4 w-full md:w-auto">
                      {/* Ngày bắt đầu */}
                      <div className="flex items-center">
                        <label className="text-sm w-1/3 md:w-auto mr-2">
                          Ngày bắt đầu thuê:
                        </label>
                        <input
                          type="date"
                          min={getTomorrowDate()}
                          value={startDate}
                          onChange={handleStartDateChange}
                          className="border rounded px-3 py-2 text-sm flex-1"
                        />
                      </div>
                      {/* Ngày kết thúc */}
                      <div className="flex items-center">
                        <label className="text-sm w-1/3 md:w-auto mr-2">
                          Ngày kết thúc thuê:
                        </label>
                        <input
                          type="date"
                          min={
                          getTomorrowDate()
                          }
                          value={endDate}
                          onChange={(e) => setEndDate(e.target.value)}
                          className="border rounded px-3 py-2 text-sm flex-1"
                        />
                      </div>
                    </div>
                  </div>

                  {/* Divider */}
                  <div className="h-px bg-gray-300 my-5"></div>
                  {/* Thành tiền */}
                  <div className="p-2 bg-gray-50 rounded-lg shadow-sm border">
                    <div className="flex items-center justify-between">
                      <h4 className="font-semibold text-gray-700">
                        Thành tiền:
                      </h4>
                      <div className="relative">
                        <p className="inline-block text-lg font-bold text-orange-600">
                          {(
                            rentalData.product.rentPrice *
                            rentalData.quantity *
                            rentalDays
                          ).toLocaleString("Vi-vn")}
                          ₫
                        </p>
                        <div
                          className="relative inline-block ml-2 cursor-pointer"
                          onMouseEnter={() => setShowTooltip(true)}
                          onMouseLeave={() => setShowTooltip(false)}
                        >
                          <FontAwesomeIcon
                            icon={faInfoCircle}
                            className="text-xl text-orange-500"
                          />
                          {showTooltip && (
                            <div className="absolute right-0 top-6 w-48 p-2 bg-white text-black text-xs rounded shadow-md border">
                              <p>
                                Giá thuê:{" "}
                                {rentalData.product.rentPrice.toLocaleString()}{" "}
                                ₫
                              </p>
                              <p>× {rentalData.quantity} (số lượng)</p>
                              <p>× {rentalDays} (ngày thuê)</p>
                              <p className="font-semibold text-orange-500">
                                {(
                                  rentalData.product.rentPrice *
                                  rentalData.quantity *
                                  rentalDays
                                ).toLocaleString()}{" "}
                                ₫
                              </p>
                            </div>
                          )}
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            {/* Phần note */}
            <div className="flex justify-between items-center pt-1 border rounded mt-4">
              <label className="block text-lg font-semibold">Lời nhắn</label>
              <input
                type="text"
                className="border rounded w-3/4 px-3 py-2 mt-2"
                value={note}
                onChange={(e) => setNote(e.target.value)}
                placeholder="Ghi chú của bạn cho 2Sport"
              />
            </div>
            <div className="h-px bg-gray-300 my-5"></div>

            {/* Phần tóm tắt giá */}
            <div className="space-y-4 py-2">
              <div className="flex justify-between items-center">
                <h3 className="text-lg font-semibold"> Tổng tiền hàng</h3>
                <p className="text-base">
                  {" "}
                  {(
                    rentalData.product.rentPrice *
                    rentalData.quantity *
                    rentalDays
                  ).toLocaleString("Vi-vn")}
                  ₫
                </p>
              </div>
              <div className="flex justify-between items-center">
                <h3 className="text-lg font-semibold">Phí giao hàng</h3>
                <p className="text-base">
                  Sẽ được{" "}
                  <span className="text-blue-700">
                    <Link to="">2Sport</Link>
                  </span>{" "}
                  thông báo lại sau
                </p>
              </div>
              <div className="flex justify-between items-center">
                <h3 className="text-lg font-semibold">Tổng thanh toán</h3>
                <p className="text-base">
                  {" "}
                  {(
                    rentalData.product.rentPrice *
                    rentalData.quantity *
                    rentalDays
                  ).toLocaleString("Vi-vn")}
                  ₫
                </p>
              </div>
            </div>
            <div className="flex items-center justify-between w-full my-4">
              <p className="text-sm w-4/6">
                Nhấn "Đặt hàng" đồng nghĩa với việc bạn đồng ý tuân theo{" "}
                <span className="text-blue-500">
                  <Link to="/second-hand-rentals">Điều khoản 2Sport</Link>
                </span>
              </p>
              <Button
                onClick={handleCreateRentalOrder}
                disabled={loading}
                className={`bg-orange-500 text-white px-6 py-3 rounded w-2/6 ${
                  loading ? "opacity-50 cursor-not-allowed" : ""
                }`}
              >
                {loading ? "Đang xử lý..." : "Đặt hàng"}
              </Button>
            </div>

            {/* <div className="flex justify-center mt-6">
              <button
                onClick={handleCreateRentalOrder}
                disabled={loading}
                className={`bg-orange-500 text-white w-full py-3 rounded ${
                  loading
                    ? "opacity-50 cursor-not-allowed"
                    : "hover:bg-orange-600"
                }`}
              >
                {loading ? "Đang tiến hành..." : "Tạo đơn hàng"}
              </button>
            </div> */}
          </div>
        </div>
      </div>
    </>
  );
};

export default RentalOrder;
