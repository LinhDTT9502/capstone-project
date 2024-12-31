import { useState, useEffect } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { selectedShipment } from "../../redux/slices/shipmentSlice";
import { useTranslation } from "react-i18next";
import { selectUser } from "../../redux/slices/authSlice";
import OrderMethod from "./OrderMethod";
import { placedOrder } from "../../services/Checkout/checkoutService";
import { addGuestOrder } from "../../redux/slices/guestOrderSlice";
import { toast, ToastContainer } from "react-toastify";
import { Button, Tooltip, Typography } from "@material-tailwind/react";
import { faInfoCircle } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

const PlacedOrder = () => {
  const user = useSelector(selectUser);
  const { t } = useTranslation();
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const shipment = useSelector(selectedShipment);
  const location = useLocation();
  const { selectedProducts } = location.state || { selectedProducts: [] };
  const [showTooltip, setShowTooltip] = useState(false);

  const [branchId, setBranchId] = useState(null);
  const [selectedOption, setSelectedOption] = useState("");
  const [orderSuccess, setOrderSuccess] = useState(false);
  const [userData, setUserData] = useState({
    fullName: "",
    gender: "",
    email: "",
    phoneNumber: "",
    address: "",
    shipmentDetailID: 0,
  });
  const [loading, setLoading] = useState(false);

  // New state for discountCode and note
  const [discountCode, setDiscountCode] = useState("");
  const [note, setNote] = useState("");

const totalPrice = selectedProducts.reduce(
  (acc, item) => acc + (item.price || 0) ,
  0
);


  const handleOptionChange = (event) => {
    setSelectedOption(event.target.value);
    // if (event.target.value === "STORE_PICKUP") {

    // }
  };

  const handleOrder = async () => {

    setLoading(true);
    try {
      if (!userData.address.trim()) {
        toast.error("Vui lòng nhập địa chỉ giao hàng.");
        return;
      }
      
      if (!userData.fullName.trim()) {
        toast.error("Vui lòng nhập họ và tên!");
        return false;
      }
      if (!/^\S+@\S+\.\S+$/.test(userData.email)) {
        toast.error("Vui lòng nhập email hợp lệ.");
        return;
      }
      if (!/^[0-9]{10}$/.test(userData.phoneNumber)) {
        toast.error("Số điện thoại phải có 10 chữ số.");
        return;
      }
      if (!userData.gender) {
        toast.error("Vui lòng chọn giới tính!");
        return false;
      }

      const token = localStorage.getItem("token");
      const data = {
        customerInformation: {
          fullName: userData.fullName,
          email: userData.email,
          gender: userData.gender,
          contactPhone: userData.phoneNumber,
          address: userData.address,
          userID: token ? user.UserId : null,
        },
        dateOfReceipt: new Date(
          Date.now() + 3 * 24 * 60 * 60 * 1000
        ).toISOString(),
        note: note,
        deliveryMethod: selectedOption,
        branchId: selectedOption === "STORE_PICKUP" ? branchId : null,

        productInformations: selectedProducts.map((item) => ({
          cartItemId: item.cartItemId || null,
          productId: item.productId,
          productName: item.productName,
          productCode: item.productCode,
          quantity: item.quantity,
          unitPrice: item.price,
          size: item.size,
          color: item.color,
          condition: item.condition,
          imgAvatarPath: item.imgAvatarPath,
        })),
        saleCosts: {
          subTotal: totalPrice,
          tranSportFee: 0,
          totalAmount: totalPrice,
        },
      };
      const response = await placedOrder(data);

      if (response) {
        if (!token) {
          dispatch(addGuestOrder(response.data));
          // console.log(response.data, "")
        }
        console.log(response)
        setOrderSuccess(true);
        navigate("/order_success", {
          state: {
            orderID: response.data.saleOrderId,
            orderCode: response.data.orderCode,
          },
        });
      }
    } catch (error) {
      console.error("Error during checkout:", error);
      toast.error("Có lỗi trong quá trình đặt hàng. Vui lòng chờ trong giây lát...");
    } finally {
      setLoading(false);
    }
  };

  //   useEffect(() => {
  //     if (orderSuccess) {
  //       navigate("/order_success");
  //     }
  //   }, [orderSuccess, navigate]);

  return (
    <>
      <ToastContainer position="top-right" autoClose={3000} />
      <div className="min-h-screen flex flex-col lg:flex-row bg-slate-200">
        {/* Cột bên trái */}
        <div className="flex-1 bg-white p-4 sm:p-6">
          <OrderMethod
            userData={userData}
            setUserData={setUserData}
            selectedOption={selectedOption}
            handleOptionChange={handleOptionChange}
            selectedBranchId={branchId}
            setSelectedBranchId={setBranchId}
            selectedProducts={selectedProducts}
          />
        </div>

        {/* Cột bên phải */}
        <div className="flex-1 bg-slate-200 p-4 sm:p-6 mt-4 lg:mt-10 overflow-y-auto">
          <div className="font-extrabold bg-white text-center p-4 sm:p-5 border rounded text-black mb-4 sm:mb-5">
            <h2 className="text-lg sm:text-xl">Thông tin đơn hàng - Đơn mua</h2>
          </div>

          {selectedProducts.length === 0 ? (
            <div className="flex justify-center items-center py-4 text-center">
              <p className="text-base sm:text-lg text-black">
                {t("checkout.no_items_selected")}
              </p>
            </div>
          ) : (
            <div className="space-y-4">
              {/* Danh sách sản phẩm */}
              <div className="max-h-96 overflow-y-auto border p-4 rounded bg-gray-50 space-y-4">
                {selectedProducts.map((item) => (
                  <div
                    key={item.id}
                    className="flex flex-col sm:flex-row items-start sm:space-x-4 border p-4 rounded bg-white"
                  >
                    {/* Ảnh sản phẩm */}
                    <div className="relative mb-4 sm:mb-0">
                      <img
                        src={item.imgAvatarPath}
                        alt={item.productName}
                        className="w-24 h-24 sm:w-32 sm:h-32 object-contain rounded"
                      />
                      <span className="absolute top-0 right-0 bg-orange-500 text-white text-xs font-bold rounded-full w-5 h-5 flex items-center justify-center">
                        {item.quantity}
                      </span>
                    </div>

                    {/* Thông tin sản phẩm */}
                    <div className="flex flex-col flex-1">
                      <div className="flex-1 pl-auto">
                        <h3 className="text-sm sm:text-lg font-semibold">
                          {item.productName}
                        </h3>
                        <ul className="text-xs sm:text-sm">
                          <li>Màu sắc: {item.color}</li>
                          <li>Kích cỡ: {item.size}</li>
                          <li>Tình trạng: {item.condition}%</li>
                          <li className="text-rose-700">
                            Đơn giá bán:{" "}
                            {(item.price / item.quantity || 0).toLocaleString(
                              "vi-VN"
                            )}
                            ₫
                          </li>
                        </ul>
                      </div>
                      <div className="h-px bg-gray-300 my-4 sm:my-5"></div>

                      {/* Thành tiền */}
                      <div className="p-2 bg-gray-50 rounded-lg shadow-sm border">
                        <div className="flex items-center justify-between">
                          <h4 className="font-semibold text-gray-700">
                            Thành tiền:
                          </h4>
                          <div className="relative">
                            <p className="inline-block text-sm sm:text-lg font-bold text-orange-600">
                              {item.price.toLocaleString("vi-VN")} ₫
                            </p>
                            {/* Tooltip */}
                            <div
                              className="relative inline-block ml-2 cursor-pointer"
                              onMouseEnter={() => setShowTooltip(true)}
                              onMouseLeave={() => setShowTooltip(false)}
                            >
                              <FontAwesomeIcon
                                icon={faInfoCircle}
                                className="text-sm sm:text-xl text-orange-500"
                              />
                              {showTooltip && (
                                <div className="absolute bottom-full left-1/2 transform -translate-x-1/2 mt-2 w-40 sm:w-48 p-2 bg-white text-black text-xs rounded shadow-md border">
                                  <p>
                                    Giá mua:{" "}
                                    {(
                                      (item.price || 0) / (item.quantity || 1)
                                    ).toLocaleString("vi-VN")}{" "}
                                    ₫
                                  </p>
                                  <p>× {item.quantity || 1} (số lượng)</p>
                                  <p className="font-semibold text-orange-500">
                                    {(
                                      (item.price || 0)
                                    ).toLocaleString("vi-VN")}{" "}
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
                ))}
              </div>

              {/* Lời nhắn */}
              <div className="flex flex-col sm:flex-row justify-between items-center pt-1 border rounded mt-4">
                <label className="block text-base sm:text-lg font-semibold">
                  Lời nhắn
                </label>
                <input
                  type="text"
                  className="border rounded w-full sm:w-3/4 px-3 py-2 mt-2 sm:mt-0"
                  value={note}
                  onChange={(e) => setNote(e.target.value)}
                  placeholder="Ghi chú của bạn cho 2Sport"
                />
              </div>
              <div className="h-px bg-gray-300 my-4 sm:my-5"></div>

              {/* Tổng tiền */}
              <div className="space-y-4 py-2">
                <div className="flex justify-between items-center">
                  <h3 className="text-sm sm:text-lg font-semibold">
                    Tổng tiền hàng
                  </h3>
                  <p className="text-sm sm:text-base">
                    {totalPrice.toLocaleString("vi-VN")}₫
                  </p>
                </div>
                <div className="flex justify-between items-center">
                  <h3 className="text-sm sm:text-lg font-semibold">
                    Phí giao hàng
                  </h3>
                  <p className="text-sm sm:text-base">
                    Sẽ được{" "}
                    <span className="text-blue-700">
                      <Link to="">2Sport</Link>
                    </span>{" "}
                    thông báo lại sau
                  </p>
                </div>
                <div className="flex justify-between items-center">
                  <h3 className="text-sm sm:text-lg font-semibold">
                    Tổng thanh toán
                  </h3>
                  <p className="text-sm sm:text-base">
                    {totalPrice.toLocaleString("vi-VN")}₫
                  </p>
                </div>
              </div>
            </div>
          )}
          {/* Đặt hàng */}
          <div className="flex flex-col sm:flex-row items-center justify-between w-full my-4">
            <p className="text-sm w-full sm:w-4/6 mb-4 sm:mb-0">
              Nhấn "Đặt hàng" đồng nghĩa với việc bạn đồng ý tuân theo{" "}
              <span className="text-blue-500">
                <Link to="/second-hand-rentals">Điều khoản 2Sport</Link>
              </span>
            </p>
            <Button
              onClick={handleOrder}
              disabled={loading}
              className={`bg-orange-500 text-white px-6 py-3 rounded w-full sm:w-2/6 ${
                loading ? "opacity-50 cursor-not-allowed" : ""
              }`}
            >
              {loading ? "Đang xử lý..." : "Đặt hàng"}
            </Button>
          </div>
        </div>
      </div>
    </>
  );
};

export default PlacedOrder;
