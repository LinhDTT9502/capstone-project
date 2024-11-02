import { useState, useEffect } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { Button } from "@material-tailwind/react";
import DeliveryAddress from "../components/Payment/DeliveryAddress";
import PaymentMethod from "../components/Payment/PaymentMethod";
import { checkout } from "../services/paymentServices";
import { useSelector } from "react-redux";
import { selectedShipment } from "../redux/slices/shipmentSlice";
import { useTranslation } from "react-i18next";
import { selectUser } from "../redux/slices/authSlice";
import { fetchBranchs } from "../services/branchService";

const Checkout = () => {
  const user = useSelector(selectUser);
  const { t } = useTranslation();
  const location = useLocation();
  const navigate = useNavigate();
  const shipment = useSelector(selectedShipment);
  const { selectedProducts } = location.state || { selectedProducts: [] };
  const [branches, setBranches] = useState([]);
  const [selectedOption, setSelectedOption] = useState(null);
  const [orderSuccess, setOrderSuccess] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [userData, setUserData] = useState({
    fullName: "",
    email: "",
    phoneNumber: "",
    address: "",
  });

  // New state for discountCode and note
  const [discountCode, setDiscountCode] = useState("");
  const [note, setNote] = useState("");

  const totalPrice = selectedProducts.reduce(
    (acc, item) => acc + item.price,
    0
  );

  // Fetch branches when component mounts
  useEffect(() => {
    const loadBranches = async () => {
      try {
        const branchData = await fetchBranchs();
        setBranches(branchData); 
      } catch (error) {
        console.error("Error fetching branches:", error);
      }
    };

    loadBranches();
  }, []);


  const getBranchIdByName = (branchName) => {
    const branch = branches.find((b) => b.branchName === branchName);
    return branch ? branch.id : null; 
  };  

  const handleOptionChange = (event) => {
    setSelectedOption(event.target.value);
  };

  const handleCheckout = async () => {
    
    try {
      const token = localStorage.getItem("token");
      const branchId = selectedProducts.length > 0 ? getBranchIdByName(selectedProducts[0].branchName) : null;
      const data = {
        orderDetailCMs: selectedProducts.map((item) => ({
          warehouseId: item.warehouseId,
          quantity: item.quantity,
          price: item.totalPrice
        })),
        branchId: branchId,
        userID: user.UserId,
        shipmentDetailID: shipment.id,
        paymentMethodID: selectedOption,
        orderType: 1,
        discountCode: discountCode || "nothing",
        note: note || "nothing",
      };

      const response = await checkout(token, data);

      if (selectedOption === "1") {
        setOrderSuccess(true);
      } else if (selectedOption === "2" && response.data.paymentLink) {
        const paymentLink = response.data.paymentLink;
        window.location.href = paymentLink;
      }
    } catch (error) {
      console.error("Error during checkout:", error);
    }
  };

  if (orderSuccess) {
    navigate("/order_success");
  }

  return (
    <div className="px-5 py-5 flex flex-row bg-slate-200">
      <div className="text-nowrap basis-2/3 bg-white mx-2 pr-14">
        <DeliveryAddress
          userData={userData}
          setUserData={setUserData}
          isEditing={isEditing}
          setIsEditing={setIsEditing}
        />
        <PaymentMethod
          selectedOption={selectedOption}
          handleOptionChange={handleOptionChange}
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
                    <p className="text-lg text-black">{item.price.toLocaleString()} VND</p>
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
            onClick={handleCheckout}
          >
            {t("checkout.complete_order")}
          </Button>
        </div>
      </div>
    </div>
  );
};

export default Checkout;
