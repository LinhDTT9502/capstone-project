import { useState } from "react";
 import { useLocation, useNavigate } from "react-router-dom";
import PaymentMethod from "../components/Payment/PaymentMethod";
import CheckoutButton from "../components/Payment/CheckoutButton";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faArrowLeft,
  faUser,
  faEnvelope,
  faPhone,
  faMapMarkerAlt,
  faShoppingCart,
  faMoneyBillWave,
  faCalendarAlt,
  faTruck,
  faBolt,
  faCircleDollarToSlot,
  faBan,
  faVenusMars,
} from "@fortawesome/free-solid-svg-icons";

const Checkout = () => {
  const location = useLocation();
  const [selectedOption, setSelectedOption] = useState(null);
  const selectedOrder = location.state?.selectedOrder || null;
   const navigate = useNavigate();

  const handleOptionChange = (event) => {
    setSelectedOption(event.target.value);
  };
  
  return (
    <div className="min-h-screen bg-gray-100 py-12 px-4 md:px-8">
      <div className="max-w-7xl mx-auto">
        {/* Page Title */}
        <div className="text-center mb-5 flex items-center justify-between">
          <button
            onClick={() => navigate(-1)}
            className="px-4 py-2 bg-gray-200 rounded hover:bg-gray-300 text-gray-700 font-medium"
          >
            Quay lại
          </button>
          <h2 className="text-orange-500 font-bold text-3xl flex-1 text-center">
            Tiến hành thanh toán
          </h2>
        </div>
        <hr className="mb-5" />

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* Customer Information */}
          <div className="bg-white p-6 shadow-lg rounded-lg">
            <h3 className="text-xl font-bold text-gray-800 mb-4">
              Thông tin khách hàng
            </h3>
            <hr className="mb-5" />
            <div className="space-y-1">
              {/* <div className="flex">
                <img
                  src={
                    selectedOrder.orderImage ||
                    "/assets/images/default_package.png"
                  }
                  alt={selectedOrder.orderImage}
                  className="w-40 h-40 object-contain rounded"
                />
              </div>
              <hr className="mb-5" /> */}
              <div className="flex">
                <p className="flex items-center gap-2 mb-2">
                  <FontAwesomeIcon icon={faUser} className="text-blue-500" />
                  <span className="font-semibold">Tên:</span>{" "}
                  <i>{selectedOrder.fullName}</i>
                </p>
              </div>
              <div className="flex">
                <p className="flex items-center gap-2 mb-2">
                  <FontAwesomeIcon
                    icon={faVenusMars}
                    className="text-blue-500 fa-xs"
                  />
                  <span className="font-semibold">Giới tính:</span>
                  <i>{selectedOrder.gender}</i>
                </p>
              </div>
              <div className="flex">
                <p className="flex items-center gap-2 mb-2">
                  <FontAwesomeIcon icon={faPhone} className="text-blue-500" />
                  <span className="font-semibold">Số điện thoại:</span>
                  <i> {selectedOrder.contactPhone}</i>
                </p>
              </div>
              <div className="flex">
                <p className="flex items-center gap-2 mb-2">
                  <FontAwesomeIcon
                    icon={faEnvelope}
                    className="text-blue-500"
                  />
                  <span className="font-semibold">Email:</span>{" "}
                  <i>{selectedOrder.email}</i>
                </p>
              </div>
              <div className="flex">
                <p className="flex items-start gap-2 mb-2">
                  <FontAwesomeIcon
                    icon={faMapMarkerAlt}
                    className="text-blue-500"
                  />
                  <span className="font-semibold flex-shrink-0">Địa chỉ:</span>
                  <span className="break-words">
                    <i>{selectedOrder.address}</i>
                  </span>
                </p>
              </div>
            </div>
          </div>

          {/* Payment method */}
          {selectedOrder && (
            <div className="bg-white p-6 shadow-lg rounded-lg">
              <h3 className="text-xl font-bold text-gray-800 mb-4">
                Phương thức thanh toán
              </h3>
              <hr className="" />
              <PaymentMethod
                selectedOption={selectedOption}
                handleOptionChange={handleOptionChange}
              />
            </div>
          )}

          {/* Order information*/}
          <div className="bg-white p-6 shadow-lg rounded-lg">
            <h3 className="text-xl font-bold text-gray-800 mb-4">
              Thông tin đơn hàng
            </h3>
            <hr className="mb-5" />
            {selectedOrder && (
              <div className="">
                <div className="space-y-4">
                  <div className="flex">
                    <p className="flex items-center gap-2 mb-2">
                      <FontAwesomeIcon
                        icon={faShoppingCart}
                        className="text-blue-500"
                      />
                      <span className="font-semibold">Mã đơn hàng:</span>{" "}
                      <i>{selectedOrder.saleOrderCode}</i>
                    </p>
                  </div>
                  <div className="flex">
                    <p className="flex items-start gap-2 mb-2 w-full">
                      <FontAwesomeIcon
                        icon={faTruck}
                        className="text-blue-500"
                      />
                      <span className="font-semibold flex-shrink-0">
                        Phương thức nhận hàng:
                      </span>
                      <span className="break-words">
                        <i>{selectedOrder.deliveryMethod}</i>
                      </span>
                    </p>
                  </div>
                  {/* <div className="flex">
                    <p className="font-medium text-gray-700 mr-2">
                      Trạng thái đơn hàng:
                    </p>
                    <p className=" text-gray-700">
                      {selectedOrder.orderStatus}
                    </p>
                  </div>
                  <div className="flex">
                    <p className="font-medium text-gray-700 mr-2">
                      Phương thức vận chuyển:{" "}
                    </p>
                    <p className=" text-gray-700">
                      {" "}
                      {selectedOrder.deliveryMethod.replace("_", " ")}
                    </p>
                  </div> */}
                </div>
                <hr className="mt-5 mb-5" />
                <h4 className="text-base font-semibold text-gray-800 mt-4 mb-2">
                  Chi tiết sản phẩm
                </h4>
                <div className="space-y-2">
                  {selectedOrder.saleOrderDetailVMs.$values.map((item) => (
                    <div key={item.productId} className="flex flex-col py-3">
                      <div className="flex flex-row items-center">
                        <img
                          src={item.imgAvatarPath}
                          alt={item.productName}
                          className="w-16 h-16 object-cover rounded-md mr-4"
                        />
                        <div>
                          <p className="font-medium text-gray-600">
                            {item.productName}
                          </p>
                          <p className="text-sm text-gray-500">
                            Màu sắc: {item.color} - Kích thước: {item.size} -
                            Tình trạng: {item.condition}%
                          </p>
                          <p className="font-medium text-sm">
                            Số lượng: {item.quantity}
                          </p>
                          <p className="font-sm text-base text-rose-700">
                            Đơn giá bán:{" "}
                            {new Intl.NumberFormat("vi-VN", {
                              style: "currency",
                              currency: "VND",
                            }).format(item.unitPrice)}
                          </p>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            )}
            <div className="mt-6 border-t pt-4">
              <div className="flex justify-between mb-2">
                <p className="font-medium text-gray-700 mr-2">Tạm tính:</p>
                <p className="font-medium text-gray-700">
                  {selectedOrder.subTotal.toLocaleString("vi-VN")}₫
                </p>
              </div>
              <div className="flex justify-between mb-2">
                <p className="font-medium text-gray-700 mr-2">
                  Phí vận chuyển:
                </p>
                <p className="font-medium text-gray-700">
                  {selectedOrder.tranSportFee.toLocaleString("vi-VN")}₫
                </p>
              </div>
              <div className="flex justify-between mb-4">
                <p className="text-lg font-semibold text-gray-800">
                  Tổng cộng:
                </p>
                <p className="text-lg font-semibold text-gray-800">
                  {selectedOrder.totalAmount.toLocaleString("vi-VN")}₫
                </p>
              </div>
            </div>

            {/* Checkout Button */}
            <div className="flex pb-10 justify-center items-center mt-4">
              <CheckoutButton
                paymentMethodID={selectedOption}
                selectedOrder={selectedOrder}
                className="mt-6 w-full bg-orange-500 text-white py-2 rounded-md"
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Checkout;
