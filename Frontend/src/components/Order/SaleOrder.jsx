import { useState, useEffect } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { Button } from "@material-tailwind/react";
import { useDispatch, useSelector } from "react-redux";
import { selectedShipment } from "../../redux/slices/shipmentSlice";
import { useTranslation } from "react-i18next";
import { selectUser } from "../../redux/slices/authSlice";
import OrderMethod from "./OrderMethod";
import { placedOrder } from "../../services/Checkout/checkoutService";
import { addGuestOrder } from "../../redux/slices/guestOrderSlice";
import { toast, ToastContainer } from "react-toastify";

const SaleOrder = () => {
  const [loading, setLoading] = useState(false);

  const user = useSelector(selectUser);
  const { t } = useTranslation();
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const shipment = useSelector(selectedShipment);
  const location = useLocation();

  // Wrap `selectedProducts` in an array if it's not already one
  const selectedProductsRaw = location.state?.selectedProducts || [];
  const selectedProducts = Array.isArray(selectedProductsRaw)
    ? selectedProductsRaw
    : [selectedProductsRaw];

  const [branchId, setBranchId] = useState(null); // Change this to null
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

  const [discountCode, setDiscountCode] = useState("");
  const [note, setNote] = useState("");

  const validateUserData = (userData, selectedOption) => {
    
  };

  // Calculate total price
  const totalPrice = selectedProducts.reduce(
    (acc, item) => acc + (item.price || 0) * (item.quantity || 1),
    0
  );

  const handleOptionChange = (event) => {
    setSelectedOption(event.target.value);
  };
  const handleOrder = async () => {
    setLoading(true);
    
    try {
      if (selectedOption === "HOME_DELIVERY" && !userData.address.trim()) {
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
        dateOfReceipt: new Date(Date.now() + 3 * 24 * 60 * 60 * 1000).toISOString(),
        note: note,
        deliveryMethod: selectedOption,
        branchId: selectedOption === "STORE_PICKUP" ? branchId : null,

        productInformations: selectedProducts.map((item) => ({
          cartItemId: null,
          productId: item.id,
          productName: item.productName,
          productCode: item.productCode,
          quantity: item.quantity || 1,
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
          console.log(response.data);
        }
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
      toast.error("Có lỗi xảy ra trong quá trình đặt hàng. Vui lòng thử lại.");
    } finally {
      setLoading(false);
    }
  };
  
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
            selectedProducts={selectedProducts}
          />
        </div>

        <div className="flex-1 bg-slate-200  p-6 overflow-y-auto mt-10">
          <div className="font-alfa bg-white text-center p-5 border rounded text-black mb-5">
            <h2 className="text-xl">{t("checkout.order_summary")}</h2>
          </div>

          {selectedProducts.length === 0 ? (
            <div className="flex justify-center items-center py-4 text-center">
              <p className="text-lg text-black">
                {t("checkout.no_items_selected")}
              </p>
            </div>
          ) : (
            <div className="space-y-4">
              {selectedProducts.map((item) => (
                <div
                  key={item.id}
                  className="flex bg-white items-center space-x-4 border p-4 rounded"
                >
                  <div className="relative">
                    <img
                      src={item.imgAvatarPath}
                      alt={item.productName}
                      className="w-32 h-32 object-contain rounded"
                    />
                    <span className="absolute top-0 right-0 bg-orange-500 text-white text-xs font-bold rounded-full w-5 h-5 flex items-center justify-center">
                      {item.quantity || 1}
                    </span>
                  </div>
                  <div className="flex-1">
                    <h3 className="text-lg font-semibold">
                      {item.productName}
                    </h3>
                    <ul className="text-sm">
                      <li>Màu sắc: {item.color}</li>
                      <li>Kích cỡ: {item.size}</li>
                      <li>Tình trạng: {item.condition}%</li>
                    </ul>
                  </div>
                  <p className="text-lg font-semibold">
                    {(
                      (item.price || 0) * (item.quantity || 1)
                    ).toLocaleString("Vi-vn")}{" "}
                    ₫
                  </p>
                </div>
              ))}
              <div className="space-y-4 py-4">
                <div className="flex justify-between items-center pt-1 border rounded mt-4">
                  <h3 className="text-lg font-semibold">
                    {t("checkout.subtotal")}
                  </h3>
                  <p className="text-lg text-black">
                    {totalPrice.toLocaleString("Vi-vn")} ₫
                  </p>
                </div>
                <div className="flex justify-between items-center pt-1 border rounded mt-4">
                  <label className="block text-lg font-semibold">Ghi chú</label>
                  <input
                    type="text"
                    className="border rounded w-3/4 px-3 py-2 mt-2"
                    value={note}
                    onChange={(e) => setNote(e.target.value)}
                    placeholder="Ghi chú của bạn"
                  />
                </div>
                <div className="h-px bg-gray-300 my-5"></div>
                <div className="flex justify-between items-center">
                  <h3 className="text-lg font-semibold">Phí giao hàng</h3>
                  <p className="text-lg">Sẽ được báo lại từ 2Sport</p>
                </div>
                <div className="flex justify-between items-center pt-1 mt-4">
                  <h3 className="text-lg font-semibold">Tổng giá</h3>
                  <p className="text-lg">{totalPrice.toLocaleString("Vi-vn")} ₫</p>
                </div>
              </div>
            </div>
          )}
          <div className="flex justify-center mt-6">
            <Button
              onClick={handleOrder}
              disabled={loading}
              className={`bg-orange-500 text-white w-full py-3 rounded ${
                loading ? "opacity-50 cursor-not-allowed" : ""
              }`}
            >
              {loading ? "Đang xử lý..." : "Tạo đơn hàng"}
            </Button>
          </div>
        </div>
      </div>
    </>
  );
};

export default SaleOrder;
