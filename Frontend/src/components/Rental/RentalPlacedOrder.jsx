import { useState, useEffect } from "react";
import axios from "axios";
import { useNavigate, useLocation, Link } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { selectUser } from "../../redux/slices/authSlice";
import OrderMethod from "../Order/OrderMethod";
import { Button } from "@material-tailwind/react";
import { toast, ToastContainer } from "react-toastify";
import { faInfoCircle } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { addGuestRentalOrder } from "../../redux/slices/guestOrderSlice";
const RentalPlacedOrder = () => {
  const user = useSelector(selectUser);
  const navigate = useNavigate();
  const location = useLocation();
  const dispatch = useDispatch();
  const [selectedProducts, setSelectedProducts] = useState(
    location.state?.selectedProducts || []
  );

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
  const [note, setNote] = useState("");
  const [loading, setLoading] = useState(false);
  const [apiResponse, setApiResponse] = useState(null);
  const [showTooltip, setShowTooltip] = useState(false);


  const token = localStorage.getItem("token");

  const getTomorrowDate = () => {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    return tomorrow.toISOString().split("T")[0];
  };

  const handleDateChange = (id, field, value) => {
    setSelectedProducts((prevProducts) =>
      prevProducts.map((product) =>
       (product.id || product.cartItemId) === id
          ? {
            ...product,
            [field]: value,
          }
          : product
      )
    );
  };

const updatedProducts = selectedProducts.map((product) => {
  const rentStartDate = product.rentalStartDate
    ? new Date(product.rentalStartDate)
    : null;
  const rentEndDate = product.rentalEndDate
    ? new Date(product.rentalEndDate)
    : null;

  const rentDays =
    rentStartDate && rentEndDate
      ? Math.floor((rentEndDate - rentStartDate) / (1000 * 60 * 60 * 24)) + 1
      : 0;
  const subTotal = product.quantity * product.rentPrice * rentDays;
  const totalPrice = product.quantity * product.rentPrice * rentDays;

  return {
    ...product,
    rentDays,
    subTotal,
    totalPrice, 
  };
});


  const subTotal = updatedProducts.reduce(
    (acc, product) => acc + product.totalPrice,
    0
  );
 
  // console.log(updatedProducts, subTotal);

  const handleCreateRentalOrder = async () => {
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
    // if (!userData.address.trim()) {
    //   toast.error("Vui lòng nhập địa chỉ giao hàng.");
    //   return;
    // }

    for (const product of updatedProducts) {
      if (!product.rentalStartDate || !product.rentalEndDate) {
        toast.error("Vui lòng nhập đầy đủ ngày bắt đầu và ngày kết thúc thuê.");
        return;
      }
      if (
        new Date(product.rentalStartDate) >= new Date(product.rentalEndDate)
      ) {
        toast.error("Ngày kết thúc phải sau ngày bắt đầu.");
        return;
      }
    }
    if (!selectedOption) {
      toast.error("Vui lòng chọn phương thức giao hàng.");
      return;
    }
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
      productInformations: updatedProducts.map((product) => ({
        cartItemId: product.cartItemId || null,
        productId: product.id,
        productCode: product.productCode,
        productName: product.productName,
        quantity: product.quantity,
        size: product.size,
        color: product.color,
        condition: product.condition,
        rentPrice: product.rentPrice,
        imgAvatarPath: product.imgAvatarPath,
        rentalDates: {
          dateOfReceipt: product.rentalStartDate,
          rentalStartDate: product.rentalStartDate,
          rentalEndDate: product.rentalEndDate,
          rentalDays: product.rentDays,
        },
        rentalCosts: {
          subTotal: product.subTotal,
          tranSportFee: 0,
          totalAmount: product.totalPrice,
        },
      })),
    };
    try {
      setLoading(true);
      const response = await axios.post(
        "https://capstone-project-703387227873.asia-southeast1.run.app/api/RentalOrder/create",
        payload,
        {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );
      console.log(response.data);
        if (!token) {
          dispatch(addGuestRentalOrder(response.data.data))
        }
        setApiResponse(response.data.data);
        navigate("/order_success");
    } catch (error) {
      console.error("Error creating rental order:", error);
    } finally {
      setLoading(false);
    }
  };
 useEffect(() => {
    
  }, [loading]);
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
        <div className="flex-1 bg-slate-200 p-6 overflow-y-auto mt-10">
          <div className="font-extrabold bg-white text-center p-5 border rounded text-black mb-5">
            <h2 className="text-xl">Thông tin đơn hàng - Đơn thuê</h2>
          </div>

          <div className="space-y-4">
            {/* Danh sách sản phẩm cuộn */}
            <div className="max-h-[65vh] overflow-y-auto space-y-4">
              {updatedProducts.map((product) => (
                <div
                  key={product.cartItemId}
                  className="p-4 rounded bg-gray-50"
                >
                  <div className="flex flex-wrap items-start space-x-4 border p-4 rounded bg-white">
                    <div className="relative w-32 h-32 flex-shrink-0">
                      <img
                        src={product.imgAvatarPath}
                        alt={product.productName}
                        className="w-full h-full object-contain rounded"
                      />
                      <span className="absolute top-0 right-0 bg-orange-500 text-white text-xs font-bold rounded-full w-5 h-5 flex items-center justify-center">
                        {product.quantity}
                      </span>
                    </div>
                    <div className="flex flex-col flex-1">
                      {/* Thông tin sản phẩm và thời gian thuê */}
                      <div className="flex flex-wrap md:flex-nowrap items-start gap-6">
                        {/* Thông tin sản phẩm */}
                        <div className="flex-1 min-w-[200px]">
                          <h3 className="text-lg font-semibold">
                            {product.productName}
                          </h3>
                          <ul className="text-sm">
                            <li>Màu sắc: {product.color}</li>
                            <li>Kích thước: {product.size}</li>
                            <li>Tình trạng: {product.condition}%</li>
                            <li className="text-rose-700">
                              Đơn giá thuê:{" "}
                              {product.rentPrice.toLocaleString("Vi-vn") ||
                                "Đang tính.."}
                              ₫/ngày
                            </li>
                          </ul>
                        </div>
                        {/* Thời gian thuê */}
                        <div className="flex flex-col space-y-4 w-full md:w-auto">
                          <div className="flex items-center">
                            <label className="text-sm w-1/3 md:w-auto mr-2">
                              Ngày bắt đầu thuê:
                            </label>
                            <input
                              type="date"
                              min={getTomorrowDate()}
                              value={product.rentalStartDate || ""}
                              onChange={(e) =>
                                handleDateChange(
                                  product.id || product.cartItemId,
                                  "rentalStartDate",
                                  e.target.value
                                )
                              }
                              className="border rounded px-3 py-2 text-sm flex-1"
                            />
                          </div>
                          <div className="flex items-center">
                            <label className="text-sm w-1/3 md:w-auto mr-2">
                              Ngày kết thúc thuê:
                            </label>
                            <input
                              type="date"
                              min={product.rentalStartDate || getTomorrowDate()}
                              value={product.rentalEndDate || ""}
                              onChange={(e) =>
                                handleDateChange(
                                  product.id || product.cartItemId,
                                  "rentalEndDate",
                                  e.target.value
                                )
                              }
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
                              {product.totalPrice.toLocaleString("Vi-vn")}₫
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
                                <div className="absolute bottom-full left-1/2 transform -translate-x-1/2 mt-2 w-48 p-2 bg-white text-black text-xs rounded shadow-md border">
                                  <p>
                                    Giá thuê:{" "}
                                    {product.rentPrice.toLocaleString("Vi-vn")}₫
                                  </p>
                                  <p>× {product.quantity} (số lượng)</p>
                                  <p>× {product.rentDays} (ngày thuê)</p>
                                  <p className="font-semibold text-orange-500">
                                    {(
                                      product.rentPrice *
                                      product.quantity *
                                      product.rentDays
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
              ))}
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
                <p className="text-base">{subTotal.toLocaleString("Vi-vn")}₫</p>
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
                <p className="text-base">{subTotal.toLocaleString("Vi-vn")}₫</p>
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
          </div>
        </div>
        <div></div>
      </div>
    </>
  );
};

export default RentalPlacedOrder;
