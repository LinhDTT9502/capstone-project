import { useState, useEffect } from "react";
import axios from "axios";
import { useNavigate, useLocation } from "react-router-dom";
import { useSelector } from "react-redux";
import { selectUser } from "../../redux/slices/authSlice";
import OrderMethod from "../Order/OrderMethod";
import { Button } from "@material-tailwind/react";

const RentalPlacedOrder = () => {
  const user = useSelector(selectUser);
  const navigate = useNavigate();
  const location = useLocation();
  const [selectedProducts, setSelectedProducts] = useState(location.state?.selectedProducts || []);
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
  // console.log(selectedProducts);
  console.log(branchId);

  console.log(selectedOption);


  const token = localStorage.getItem("token");

  const getTomorrowDate = () => {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    return tomorrow.toISOString().split("T")[0];
  };

  const handleDateChange = (cartItemId, field, value) => {
    setSelectedProducts((prevProducts) =>
      prevProducts.map((product) =>
        product.cartItemId === cartItemId
          ? {
            ...product,
            [field]: value,
          }
          : product
      )
    );
    console.log(selectedProducts);

  };

  // Calculate fields for each product
  const updatedProducts = selectedProducts.map(product => {
    const rentStartDate = new Date(product.rentalStartDate);
    const rentEndDate = new Date(product.rentalEndDate);

    // Calculate rentDays
    const rentDays = Math.floor((rentEndDate - rentStartDate) / (1000 * 60 * 60 * 24)) + 1;

    // Calculate totalPrice
    const totalPrice = product.quantity * product.rentPrice * rentDays;

    // Return product with calculated fields
    return {
      ...product,
      rentDays,
      totalPrice,
    };
  });

  // Calculate subTotal
  const subTotal = updatedProducts.reduce((acc, product) => acc + product.totalPrice, 0);

  // console.log(updatedProducts, subTotal);


  const handleCreateRentalOrder = async () => {

    if (!selectedProducts || selectedProducts.length === 0) {
      alert("No product selected for rental.");
      return;
    }

    const payload = {
      customerInformation: {
        userId: token ? user.UserId : 0,
        email: userData.email,
        fullName: userData.fullName,
        gender: userData.gender,
        contactPhone: userData.phoneNumber,
        address: userData.address,
      },
      note: note,
      deliveryMethod: selectedOption,
      branchId: selectedOption === "STORE_PICKUP" ? branchId : null,
      productInformations: updatedProducts.map(product => ({
        cartItemId: product.cartItemId,
        productId: product.productId,
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
          subTotal: product.totalPrice,
          tranSportFee: 0,
          totalAmount: product.totalPrice,
        },
      })),
    };
    console.log(payload);

    try {
      setLoading(true);
      const response = await axios.post(
        "https://capstone-project-703387227873.asia-southeast1.run.app/api/RentalOrder/create",
        payload,
        {
          headers: {
            "Authorization": `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );
      setApiResponse(response.data.data);
      navigate("/order_success", {
        state: {
          orderID: response.data.data.id,
          orderCode: response.data.data.rentalOrderCode,
        },
      });
      alert("Rental order created successfully!");
    } catch (error) {
      console.error("Error creating rental order:", error);
      alert("Failed to create rental order.");
    } finally {
      setLoading(false);
    }
  };

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
      <div className="flex-1 bg-slate-200 p-6 overflow-y-auto mt-10">
        <div className="font-alfa bg-white text-center p-5 border rounded text-black mb-5">
          <h2 className="text-xl">Tóm tắt đơn hàng</h2>
        </div>
        
        <div className="space-y-4">
          {updatedProducts.map((product) => (
            <div key={product.cartItemId} className="flex bg-white items-center space-x-4 border p-4 rounded">
              <div className="relative">
                <img
                  src={product.imgAvatarPath}
                  alt={product.productName}
                  className="w-32 h-32 object-contain rounded"
                />
                <span className="absolute top-0 right-0 bg-orange-500 text-white text-xs font-bold rounded-full w-5 h-5 flex items-center justify-center">
                  {product.quantity}
                </span>
              </div>
              <div className="flex-1">
                <h3 className="text-lg font-semibold">{product.productName}</h3>
                <ul className="text-sm">
                  <li>Màu sắc: {product.color}</li>
                  <li>Kích thước: {product.size}</li>
                  <li>Tình trạng: {product.condition}%</li>
                  <li>Giá thuê: {product.rentPrice.toLocaleString()} ₫/ngày</li>
                </ul>
                <div className="mt-2 space-y-2">
                  <div className="flex items-center space-x-2">
                    <label className="text-sm">Ngày bắt đầu:</label>
                    <input
                      type="date"
                      min={getTomorrowDate()}
                      value={product.rentalStartDate || ""}
                      onChange={(e) => handleDateChange(product.cartItemId, 'rentalStartDate', e.target.value)}
                      className="border rounded px-2 py-1 text-sm"
                    />
                  </div>
                  <div className="flex items-center space-x-2">
                    <label className="text-sm">Ngày kết thúc:</label>
                    <input
                      type="date"
                      min={product.rentalStartDate || getTomorrowDate()}
                      value={product.rentalEndDate || ""}
                      onChange={(e) => handleDateChange(product.cartItemId, 'rentalEndDate', e.target.value)}
                      className="border rounded px-2 py-1 text-sm"
                    />
                  </div>
                </div>
              </div>
              <p className="text-lg font-semibold">{product.totalPrice.toLocaleString()} ₫</p>
            </div>
          ))}
          
          <div className="space-y-4 px-6 py-4">
            <div className="flex justify-between items-center">
              <h3 className="text-lg font-semibold">Tạm tính</h3>
              <p className="text-lg">{subTotal.toLocaleString()} ₫</p>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700">Ghi chú:</label>
              <input
                type="text"
                className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-orange-500 focus:border-orange-500 sm:text-sm"
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
              <p className="text-lg font-bold">{subTotal.toLocaleString()} ₫</p>
            </div>
          </div>
          <div className="flex justify-center mt-6">
            <Button
              onClick={handleCreateRentalOrder}
              className={`bg-orange-500 text-white w-full py-3 rounded ${loading ? "opacity-50" : ""}`}
            >
              {loading ? "Đang tiến hành..." : "Tạo đơn hàng"}
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default RentalPlacedOrder;