import { useState, useEffect } from "react";
import axios from "axios";
import { useNavigate, useLocation } from "react-router-dom";
import { useSelector } from "react-redux";
import { selectUser } from "../../redux/slices/authSlice";
import OrderMethod from "../Order/OrderMethod";

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

  return (<>
    <div className="flex flex-row bg-slate-200">
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
      <div className="basis-3/5  pr-20 pl-5 h-1/4 mt-10">
        <div className="font-alfa text-center p-5 border rounded text-black">
          Tóm tắt đơn hàng
        </div>
        <div className="overflow-auto h-3/4">
          <div className="grid grid-cols-1 gap-4">
            {updatedProducts.map((product) => (
              <div key={product.cartItemId} className=" border rounded p-4 space-x-2">
                <div className="flex">
                  <div className="relative bg-white mr-4">
                    <img
                      src={product.imgAvatarPath}
                      alt={product.productName}
                      className="w-32 h-32 object-contain rounded"
                    />
                    <span className="absolute top-0 right-0 bg-orange-500 text-white text-xs font-bold rounded-full w-5 h-5 flex items-center justify-center">
                      {product.quantity}
                    </span>
                  </div>
                  <div className="flex justify-between w-full">
                    <div className="flex flex-col space-y-4 text-wrap mr-2">
                      <h3 className="text-lg font-semibold">
                        {product.productName}
                      </h3>
                      <div className="text-sm">
                        <li>Màu sắc: {product.color}</li>
                        <li>Kích cỡ: {product.size}</li>
                        <li>Tình trạng: {product.condition}%</li>
                        <li>Giá thuê: {product.rentPrice.toLocaleString('vi-VN')}</li>
                      </div>
                    </div>
                    <p className="text-lg text-black text-center flex items-center justify-center ">{(product.totalPrice).toLocaleString('vi-VN')} </p>
                  </div>

                </div>
                {/* Date part */}
                <div className="flex bg-blue-200 text-slate-600 p-2">
                  <label>Ngày bắt đầu thuê:</label>
                  <input
                    type="date"
                    min={getTomorrowDate()}
                    value={product.rentalStartDate || ""}
                    onChange={(e) => handleDateChange(product.cartItemId, 'rentalStartDate', e.target.value)}
                    className="rounded-lg"
                  />
                  <label>Ngày kết thúc thuê:</label>
                  <input
                    type="date"
                    min={product.rentalStartDate || getTomorrowDate()}
                    value={product.rentalEndDate || ""}
                    onChange={(e) => handleDateChange(product.cartItemId, 'rentalEndDate', e.target.value)}
                  />
                </div>
              </div>
            ))}
          </div>

          <div className="text-red-700 flex justify-end text-sm font-bold my-2">* Đơn vị tiền tệ: ₫</div>
          <div className="h-px bg-gray-300 mx-auto font-bold"></div>
          <div className="flex justify-between items-center pt-1 border rounded mt-4">
            <h3 className="text-lg font-semibold">
              Tạm tính
            </h3>
            <p className="text-lg text-black">
              {subTotal.toLocaleString()}
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
          <div className="flex justify-between items-center pt-1 border rounded mt-4">
            <h3 className="text-lg font-semibold">
              Phí vận chuyển
            </h3>
            <p className="text-lg text-black">
              2Sport sẽ liên hệ và thông báo sau
            </p>
          </div>
          <div className="flex justify-between items-center pt-1 border rounded mt-4">
            <h3 className="text-lg font-semibold">Tổng cộng</h3>
            <p className="text-lg text-black">
              {subTotal.toLocaleString()}
            </p>
          </div>
        </div>
        <div className="flex pb-10 justify-center items-center">
          <button
            onClick={handleCreateRentalOrder}
            disabled={loading}
            className="bg-orange-500 text-white px-4 py-2 rounded-md"
          >
            {loading ? "Processing..." : "Create Rental Order"}
          </button>
        </div>
      </div>
    </div>
  </>
  );
};

export default RentalPlacedOrder;