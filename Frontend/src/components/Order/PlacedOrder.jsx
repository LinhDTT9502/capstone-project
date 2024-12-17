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
  const navigate = useNavigate();
  const shipment = useSelector(selectedShipment);
  const location = useLocation();
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
    (acc, item) => acc + item.price ,
    0
  );

  const handleOptionChange = (event) => {
    setSelectedOption(event.target.value);
    // if (event.target.value === "STORE_PICKUP") {

    // }
  };

  const handleOrder = async () => {
    console.log(userData);
    console.log(selectedProducts);
    

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
        dateOfReceipt: new Date(Date.now() + 3 * 24 * 60 * 60 * 1000).toISOString(),
        note: note,
        deliveryMethod: selectedOption,
        branchId: selectedOption === "STORE_PICKUP" ? branchId : null,
        productInformations: selectedProducts.map(item => ({
          cartItemId: item.cartItemId || null,
          productId: item.productId,
          productName: item.productName,
          productCode:item.productCode,
          quantity: item.quantity,
          unitPrice: item.price,
          size: item.size,
          color: item.color,
          condition: item.condition,
          imgAvatarPath: item.imgAvatarPath,
        })),
        saleCosts: {
          subTotal: totalPrice ,
          tranSportFee: 0,
          totalAmount: totalPrice,
        },
      };
console.log(data);

      // Call the placedOrder function to make the API request
      const response = await placedOrder(data);

      if (response) {
        // console.log(response);
        // console.log(response.data);


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
    <div className="flex flex-row bg-slate-200 ">
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
      <div className="basis-3/5 pr-20 pl-5 h-1/4 mt-10 pb-10">
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
                <div key={item.id} className="flex border rounded  space-x-2">
                  <div className="relative">
                    <div className="bg-white">
                      <img
                        src={item.imgAvatarPath}
                        alt={item.productName}
                        className="h-32 w-48 object-contain rounded"
                      />
                    </div>
                    <span className="absolute top-0 right-0 bg-orange-500 text-white text-xs font-bold rounded-full w-5 h-5 flex items-center justify-center">
                      {item.quantity}
                    </span>
                  </div>
                  <div className="flex justify-between w-full">
                    <div className="flex flex-col space-y-4">
                      <h3 className="text-lg font-semibold">
                        {item.productName}
                      </h3>
                      <div className="text-sm">
                        <li>Màu sắc: {item.color}</li>
                        <li>Kích cỡ: {item.size}</li>
                        <li>Tình trạng: {item.condition}%</li>
                      </div>
                    </div>
                    <p className="text-lg text-black">{(item.price).toLocaleString()} ₫</p>
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
                {totalPrice.toLocaleString()} ₫
              </p>
            </div>
            {/* <div className="flex justify-between items-center pt-1 border rounded mt-4">
              <label className="block text-lg font-semibold">Mã ưu đãi</label>
              <input
                type="text"
                className="border rounded w-3/4 px-3 py-2 mt-2"
                value={discountCode}
                onChange={(e) => setDiscountCode(e.target.value)}
                placeholder="nhập mã ưu đãi tại đây"
              />
            </div> */}
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
                {totalPrice.toLocaleString()} ₫
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
