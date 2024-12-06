import { useState } from "react";
import { useLocation } from "react-router-dom";
import PaymentMethod from "../components/Payment/PaymentMethod";
import CheckoutButton from "../components/Payment/CheckoutButton";
import {
  Card,
  CardHeader,
  CardBody,
  Typography,
  List,
  ListItem,
} from "@material-tailwind/react";

const Checkout = () => {
  const location = useLocation();
  const [selectedOption, setSelectedOption] = useState(null);
  const selectedOrder = location.state?.selectedOrder || null;

  const handleOptionChange = (event) => {
    setSelectedOption(event.target.value);
  };

  const formatCurrency = (amount) => {
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
    }).format(amount);
  };

  return (
    <div className="min-h-screen bg-gray-100 py-8 px-4 md:px-8">
      <div className="max-w-7xl mx-auto">
        {/* Page Title */}
        <div className="text-center mb-5">
          <h2 className="text-orange-500 font-bold text-3xl">
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
            <div className="space-y-4">
              <div className="flex">
                <p className="font-medium text-gray-700 mr-2">Người nhận:</p>
                <p className=" text-gray-700">
                  {selectedOrder.fullName} ({selectedOrder.gender})
                </p>
              </div>
              <div className="flex">
                <p className="font-medium text-gray-700 mr-2">Số điện thoại:</p>
                <p className=" text-gray-700">{selectedOrder.contactPhone} </p>
              </div>
              <div className="flex">
                <p className="font-medium text-gray-700 mr-2">Email:</p>
                <p className=" text-gray-700">{selectedOrder.email}</p>
              </div>
              <div className="flex">
                <p className="font-medium text-gray-700 mr-1">Địa chỉ:</p>
                <p className=" text-gray-700">{selectedOrder.address}</p>
              </div>
              <div className="flex">
                <p className="font-medium text-gray-700 mr-2">
                  Dự kiến giao hàng:{" "}
                </p>
                <p className=" text-gray-700">
                  {new Date(selectedOrder.dateOfReceipt).toLocaleDateString()}
                </p>
              </div>
            </div>
          </div>

          {/* Payment Method */}
          <div className="bg-white p-6 shadow-lg rounded-lg">
            <h3 className="text-xl font-bold text-gray-800 mb-4">
              Phương thức thanh toán
            </h3>
            <hr className="mb-5" />
            <PaymentMethod
              selectedOption={selectedOption}
              handleOptionChange={handleOptionChange}
            />
          </div>

          {/* Order Summary */}
          {selectedOrder && (
            <div className="bg-white p-6 shadow-lg rounded-lg">
              <h3 className="text-xl font-bold text-gray-800 mb-4">
                Tóm tắt đơn hàng
              </h3>
              <hr className="mb-5" />
              <div className="space-y-4">
                <div className="flex">
                  <p className="font-medium text-gray-700 mr-2">Mã đơn hàng:</p>
                  <p className=" text-gray-700">
                    {selectedOrder.saleOrderCode}-{selectedOrder.id}
                  </p>
                </div>
                <div className="flex">
                  <p className="font-medium text-gray-700 mr-2">
                    Trạng thái đơn hàng:
                  </p>
                  <p className=" text-gray-700">{selectedOrder.orderStatus}</p>
                </div>
                <div className="flex">
                  <p className="font-medium text-gray-700 mr-2">
                    Phương thức vận chuyển:{" "}
                  </p>
                  <p className=" text-gray-700">
                    {" "}
                    {selectedOrder.deliveryMethod.replace("_", " ")}
                  </p>
                </div>
              </div>
              <hr className="mt-5 mb-5" />

              <h4 className="text-lg font-semibold text-gray-800 mt-4 mb-2">
                Chi tiết đơn hàng
              </h4>
              <div className="space-y-4">
                {selectedOrder.saleOrderDetailVMs.$values.map((item) => (
                  <div key={item.productId} className="flex flex-col py-3">
                    <div className="flex flex-row items-center">
                      <img
                        src={item.imgAvatarPath}
                        alt={item.productName}
                        className="w-16 h-16 object-cover rounded-md mr-4"
                      />
                      <div>
                        <p className="font-medium text-gray-800">
                          {item.productName}
                        </p>
                        <p className="text-xs text-gray-500">
                          {item.color} - {item.size} - {item.condition}
                        </p>
                        <p className="text-sm text-gray-500">
                          x{item.quantity}
                        </p>
                        <p className="font-medium text-gray-700 flex flex-col items-end">
                          {formatCurrency(item.unitPrice)}
                        </p>
                      </div>
                    </div>
                  </div>
                ))}
              </div>

              <div className="mt-6 border-t pt-4">
                <div className="flex justify-between mb-2">
                  <p className="font-medium text-gray-700 mr-2">
                    Tổng giá trị đơn hàng:
                  </p>
                  <p className="font-medium text-gray-700">
                    {formatCurrency(selectedOrder.subTotal)}
                  </p>
                </div>
                <div className="flex justify-between mb-2">
                  <p className="font-medium text-gray-700 mr-2">
                    Phí vận chuyển:
                  </p>
                  <p className="font-medium text-gray-700">
                    {formatCurrency(selectedOrder.tranSportFee)}{" "}
                  </p>
                </div>
                <div className="flex justify-between mb-4">
                  <p className="text-lg font-semibold text-gray-800">
                    Tổng cộng:
                  </p>
                  <p className="text-lg font-semibold text-gray-800">
                    {formatCurrency(selectedOrder.totalAmount)}
                  </p>
                </div>
              </div>

              {/* Checkout Button */}
              <div className="flex flex-col justify-between items-end">
                <CheckoutButton
                  paymentMethodID={selectedOption}
                  selectedOrder={selectedOrder}
                  className="mt-6 w-full bg-orange-500 text-white py-2 rounded-md"
                />
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default Checkout;
