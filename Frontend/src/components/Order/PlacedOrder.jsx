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

const PlacedOrder = () => {
  const user = useSelector(selectUser);
  const { t } = useTranslation();
  const dispatch = useDispatch();
  const location = useLocation();
  const navigate = useNavigate();
  const shipment = useSelector(selectedShipment);
  const { selectedProducts } = location.state || { selectedProducts: [] };
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

  // New state for discountCode and note
  const [discountCode, setDiscountCode] = useState("");
  const [note, setNote] = useState("");

  const totalPrice = selectedProducts.reduce(
    (acc, item) => acc + item.price * item.quantity, // Calculate total price correctly
    0
  );

  const handleOptionChange = (event) => {
    setSelectedOption(event.target.value);
    if (event.target.value === "STORE_PICKUP") {
      // Logic to handle branch selection can be added here if necessary
    }
  };

  const handleOrder = async () => {
    try {
      const token = localStorage.getItem("token");
      const data = {
        fullName: userData.fullName,
        email: userData.email,
        contactPhone: userData.phoneNumber,
        address: userData.address,
        userID: token ? user.UserId : 0, // If user is not logged in, userID is 0
        shipmentDetailID: userData.shipmentDetailID, 
        deliveryMethod: selectedOption,
        gender: userData.gender,
        branchId: selectedOption === "STORE_PICKUP" ? branchId : null,
        dateOfReceipt: null,
        discountCode: discountCode || null,
        note: note || null,
        saleOrderDetailCMs: selectedProducts.map((item) => ({
          productId: item.id, // Assuming the item has an id
          productName: item.productName, // Assuming the item has a productName
          quantity: item.quantity,
          unitPrice: item.price,
        })),
      };

      // Call the placedOrder function to make the API request
      const response = await placedOrder(data);

      if (response) {
        console.log(response);
        console.log(response.data);
        
  
        // Check if user is a guest (no token)
        if (!token) {
          // Save the response to Redux store for guest users
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

//   useEffect(() => {
//     if (orderSuccess) {
//       navigate("/order_success");
//     }
//   }, [orderSuccess, navigate]);

  return (
    <div className="px-5 py-5 flex flex-row bg-slate-200">
      <div className="text-nowrap basis-2/3 bg-white mx-2 pr-14">
        <OrderMethod
          userData={userData}
          setUserData={setUserData}
          selectedOption={selectedOption}
          handleOptionChange={handleOptionChange}
          selectedBranchId={branchId} 
          setSelectedBranchId={setBranchId}
        />
      </div>
      <div className="basis-3/5 mx-2 h-1/4">
        <div className="font-alfa text-center p-5 border rounded text-black">
          {t("checkout.order_summary")}
        </div>
        {selectedProducts.length === 0 ? (
          <div className="flex justify-center items-center py-4 text-center">
            <p className="text-lg text-black">
              {t("checkout.no_items_selected")}
            </p>
          </div>
        ) : (
          <div className="overflow-auto h-3/4">
            <div className="grid grid-cols-1 gap-4">
              {selectedProducts.map((item) => (
                <div key={item.id} className="flex border rounded p-4 space-x-2">
                  <div className="relative">
                    <img
                      src={item.imgAvatarPath}
                      alt={item.productName}
                      className="w-auto h-32 object-scale-down rounded"
                    />
                    <span className="absolute top-0 right-0 bg-orange-500 text-white text-xs font-bold rounded-full w-5 h-5 flex items-center justify-center">
                      {item.quantity}
                    </span>
                  </div>
                  <div className="flex justify-between w-full">
                    <div className="flex flex-col space-y-4">
                      <h3 className="text-lg font-semibold w-60">
                        {item.productName}
                      </h3>
                    </div>
                    <p className="text-lg text-black">{(item.price * item.quantity).toLocaleString()} VND</p>
                  </div>
                </div>
              ))}
            </div>
            <div className="h-px bg-gray-300 my-5 mx-auto font-bold"></div>
            <div className="flex justify-between items-center pt-1 border rounded mt-4">
              <h3 className="text-lg font-semibold">
                {t("checkout.subtotal")}
              </h3>
              <p className="text-lg text-black">
                {totalPrice.toLocaleString()} VND
              </p>
            </div>
            <div className="flex justify-between items-center pt-1 border rounded mt-4">
              <label className="block text-lg font-semibold">Mã ưu đãi</label>
              <input
                type="text"
                className="border rounded w-3/4 px-3 py-2 mt-2"
                value={discountCode}
                onChange={(e) => setDiscountCode(e.target.value)}
                placeholder="nhập mã ưu đãi tại đây"
              />
            </div>
            <div className="flex justify-between items-center pt-1 border rounded mt-4">
              <label className="block text-lg font-semibold">Ghi chú</label>
              <input
                type="text"
                className="border rounded w-3/4 px-3 py-2 mt-2"
                value={note}
                onChange={(e) => setNote(e.target.value)}
                placeholder="ghi chú của bạn"
              />
            </div>
            <div className="flex justify-between items-center pt-1 border rounded mt-4">
              <h3 className="text-lg font-semibold">
                {t("checkout.transport_fee")}
              </h3>
              <p className="text-lg text-black">
                2Sport sẽ liên hệ và thông báo sau
              </p>
            </div>
            <div className="flex justify-between items-center pt-1 border rounded mt-4">
              <h3 className="text-lg font-semibold">{t("checkout.total")}</h3>
              <p className="text-lg text-black">
                {totalPrice.toLocaleString()} VND
              </p>
            </div>
          </div>
        )}
        <div className="flex justify-center items-center">
          <Button
            className="text-white bg-orange-500 w-40 py-3 rounded"
            onClick={handleOrder}
          >
            {t("checkout.complete_order")}
          </Button>
        </div>
      </div>
    </div>
  );
};

export default PlacedOrder;
