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

  // Calculate total price
  const totalPrice = selectedProducts.reduce(
    (acc, item) => acc + (item.price || 0) * (item.quantity || 1),
    0
  );

  const handleOptionChange = (event) => {
    setSelectedOption(event.target.value);
  };

  const handleOrder = async () => {
    try {
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
    }
  };

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
          <h2 className="text-xl">{t("checkout.order_summary")}</h2>
        </div>

        {selectedProducts.length === 0 ? (
          <div className="flex justify-center items-center py-4 text-center">
            <p className="text-lg text-black">
              {t("checkout.no_items_selected")}
            </p>
          </div>
        ) : (
          <div className="space-y-4 ">
            <div className="flex bg-white items-center space-x-4 border p-4 rounded">
              {selectedProducts.map((item) => (
                <div key={item.id} className="flex rounded  space-x-2">
                  <div className="relative">
                    <div className="bg-white">
                      <img
                        src={item.imgAvatarPath}
                        alt={item.productName}
                        className="w-32 h-32 object-contain rounded"
                      />
                    </div>
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
                    ).toLocaleString()}{" "}
                    ₫
                  </p>
                </div>
              ))}
            </div>
            <label className="block text-sm">Ghi chú:</label>
              <input
                type="text"
                className="border rounded w-full px-4 py-2 mt-2"
                value={note}
                onChange={(e) => setNote(e.target.value)}
                placeholder="Your notes here"
              />
            {/* Add additional sections like subtotal, notes, etc. */}
          </div>
        )}
        <div className="flex justify-center mt-6">
          <button
            onClick={handleOrder}
            disabled={loading}
            className={`bg-orange-500 text-white w-full py-3 rounded ${
              loading ? "opacity-50" : ""
            }`}
          >
            {loading ? "Đang tiến hành..." : "Tạo đơn hàng"}
          </button>
        </div>
      </div>
    </div>
  );
};

export default SaleOrder;
