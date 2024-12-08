import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { fetchProductByProductCode } from "../services/productService";
import { useTranslation } from "react-i18next";
import AddToCart from "../components/Product/AddToCart";
import RentalButton from "../components/Rental/RentalButton";
import { ProductColor } from "../components/Product/ProductColor";
import { ProductSize } from "../components/Product/ProductSize";
import { ProductCondition } from "../components/Product/ProductCondition";
import { checkQuantityProduct } from "../services/warehouseService";
import { getComment } from "../services/Comment/CommentService";
import CommentList from "../components/Comment/CommentList";
import LikeButton from "../components/Product/LikeButton";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSpinner, faGift, faMinus, faPlus, faShoppingCart, faMoneyBillWave } from "@fortawesome/free-solid-svg-icons";

const ProductDetails = () => {
  const { productCode } = useParams();
  const [product, setProduct] = useState(null);
  const [displayImage, setDisplayImage] = useState("");
  const [selectedColor, setSelectedColor] = useState("");
  const [selectedSize, setSelectedSize] = useState("");
  const [selectedCondition, setSelectedCondition] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const { t } = useTranslation();
  const [quantity, setQuantity] = useState(1);
  const navigate = useNavigate();
  const [warning, setWarning] = useState("");
  const [isFormValid, setIsFormValid] = useState(false);
  const [comments, setComments] = useState([]);

  useEffect(() => {
    const getProduct = async () => {
      try {
        const productData = await fetchProductByProductCode(
          productCode,
          selectedColor,
          selectedSize,
          selectedCondition
        );
        if (productData.length > 0) {
          setProduct(productData[0]);
          setDisplayImage(productData[0].imgAvatarPath);
        }
        setLoading(false);
      } catch (error) {
        setError(error);
        setLoading(false);
      }
    };

    getProduct();
  }, [productCode, selectedColor, selectedSize, selectedCondition]);

  useEffect(() => {
    const fetchImage = async () => {
      try {
        const productData = await fetchProductByProductCode(
          productCode,
          selectedColor,
          "",
          ""
        );
        if (productData.length > 0) {
          setDisplayImage(productData[0].imgAvatarPath);
        }
      } catch (error) {
        console.error("Error fetching image:", error);
      }
    };

    if (selectedColor) {
      fetchImage();
    }
  }, [selectedColor, productCode]);

  const handleRentalClick = () => {
    if (!selectedColor || !selectedSize || !selectedCondition) {
      alert("Vui lòng chọn màu sắc, kích cỡ và tình trạng của sản phẩm!");
    } else {
      const rentalData = { product, quantity };
      localStorage.setItem("rentalData", JSON.stringify(rentalData));
      navigate("/rental-order");
    }
  };

  const handlePlaceOrder = async () => {
    if (!selectedColor || !selectedSize || !selectedCondition) {
      alert("Vui lòng chọn màu sắc, kích cỡ và tình trạng của sản phẩm!");
    } else {
      const response = await checkQuantityProduct(product.id);
      if (quantity <= response.availableQuantity) {
        navigate("/sale-order", { state: { selectedProducts: product } });
      } else {
        alert(
          `Sản phẩm này chỉ còn lại ${response.availableQuantity} sản phẩm trong kho`
        );
      }
    }
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center h-screen flex-col space-y-20">
        <FontAwesomeIcon icon={faSpinner} spin size="3x" color="#ff6800" />
        <p>Đang tải chi tiết sản phẩm, vui lòng chờ một chút...</p>
      </div>
    );
  }

  if (error) {
    return <div>{t("product_details.error")}</div>;
  }
y
  return (
    <div className="container mx-auto px-4 md:px-20 py-10 bg-white rounded-lg shadow-lg">
      {product && (
        <>
          <div className="flex flex-col md:flex-row gap-8 justify-between">
            <div className="md:w-2/5">
              <img
                src={displayImage}
                alt={product.imgAvatarName || "Product Image"}
                className="w-full object-contain rounded-lg"
              />
              <div className="flex flex-wrap mt-4 gap-2">
                {product.listImages?.$values.map((image, index) => (
                  <img
                    key={index}
                    src={image}
                    alt={`Thumbnail ${index + 1}`}
                    onClick={() => setDisplayImage(image)}
                    className={`w-20 h-20 object-contain border-2 rounded-md cursor-pointer ${
                      displayImage === image
                        ? "border-orange-500"
                        : "border-gray-300"
                    }`}
                  />
                ))}
              </div>
            </div>
            <div className="md:w-1/2">
              <h4 className="text-lg text-orange-500">
                {product.categoryName}
              </h4>
              <h2 className="text-3xl font-bold text-black mt-2">
                {product.productName}
              </h2>

              <div className="my-4 text-gray-800 space-y-2">
                <p>
                  <strong>Thương hiệu:</strong> {product.brandName}
                </p>
                <p>
                  <strong>Mã sản phẩm:</strong> {product.productCode}
                </p>
                <ProductColor
                  productCode={productCode}
                  selectedColor={selectedColor}
                  setSelectedColor={setSelectedColor}
                />
                <ProductSize
                  productCode={productCode}
                  color={selectedColor}
                  selectedSize={selectedSize}
                  setSelectedSize={setSelectedSize}
                />
                <ProductCondition
                  productCode={productCode}
                  color={selectedColor}
                  size={selectedSize}
                  selectedCondition={selectedCondition}
                  setSelectedCondition={setSelectedCondition}
                />
                <p>
                  <strong>Giá:</strong>{" "}
                  {product.price
                    ? `${product.price.toLocaleString()} ₫`
                    : "N/A"}
                </p>
                <p>
                  <strong>Giá thuê:</strong>{" "}
                  {product.rentPrice
                    ? `${product.rentPrice.toLocaleString()} ₫`
                    : "Sản phẩm chỉ bán"}
                </p>
                <div className="flex items-center space-x-4">
                  <div className="flex items-center space-x-2">
                    <button
                      onClick={() => setQuantity((prev) => Math.max(1, prev - 1))}
                      className="px-3 py-1 bg-orange-500 text-white rounded-md"
                    >
                      <FontAwesomeIcon icon={faMinus} />
                    </button>
                    <input
                      type="number"
                      value={quantity}
                      onChange={(e) =>
                        setQuantity(Math.max(1, Number(e.target.value)))
                      }
                      className="w-16 h-8 text-center text-sm p-1 border rounded"
                      min="1"
                    />
                    <button
                      onClick={() => setQuantity((prev) => prev + 1)}
                      className="px-3 py-1 bg-orange-500 text-white rounded-md"
                    >
                      <FontAwesomeIcon icon={faPlus} />
                    </button>
                  </div>
                  <LikeButton
                    productId={product.id}
                    initialLikes={product.likes}
                    isLikedInitially={product.isLiked}
                  />
                </div>
              </div>
              <div className="space-y-3 w-full">
                <div className="flex items-center mt-4 space-x-4 w-full">
                  <button
                    onClick={handlePlaceOrder}
                    className="flex-1 px-4 py-2 bg-orange-500 text-white rounded-md hover:bg-orange-600 transition-colors"
                  >
                    <FontAwesomeIcon icon={faShoppingCart} className="mr-2" />
                    Mua ngay
                  </button>
                  <button
                    onClick={handleRentalClick}
                    disabled={!product.isRent}
                    className="flex-1 px-4 py-2 bg-rose-700 text-white rounded-md hover:bg-rose-800 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    <FontAwesomeIcon icon={faMoneyBillWave} className="mr-2" />
                    Thuê ngay
                  </button>
                </div>
                <AddToCart
                  product={product}
                  quantity={quantity}
                  selectedColor={selectedColor}
                  selectedSize={selectedSize}
                  selectedCondition={selectedCondition}
                />
              </div>
              <div className="relative mt-6 border border-orange-400 rounded-lg p-4 bg-orange-50">
                <h3 className="absolute -top-3 left-4 bg-orange-100 px-2 py-1 text-orange-600 font-semibold text-sm border border-orange-500 rounded-md">
                  <FontAwesomeIcon icon={faGift} className="mr-2" />
                  ƯU ĐÃI
                </h3>
                <ul className="list-disc ml-5 space-y-2 text-gray-800 mt-2">
                  <li>
                    Tặng 1 đôi vớ cầu lông (vớ{" "}
                    <span className="text-orange-600 font-semibold">
                      dài nhiều màu
                    </span>{" "}
                    hoặc{" "}
                    <span className="text-orange-600 font-semibold">
                      vớ ngắn
                    </span>
                    )
                  </li>
                  <li>Sản phẩm cam kết chính hãng</li>
                  <li>Thanh toán sau khi kiểm tra và nhận hàng</li>
                  <li>
                    Bảo hành chính hãng theo nhà sản xuất
                    <span className="text-gray-500">
                      {" "}
                      (Trừ hàng nội địa, xách tay)
                    </span>
                  </li>
                </ul>
              </div>
            </div>
          </div>
          <div className="mt-8">
            <h3 className="font-poppins text-lg text-orange-500 font-bold mb-2">
              Mô tả sản phẩm:
            </h3>
            <p className="text-gray-700">{product.description}</p>
          </div>
          <CommentList productId={product?.id} />
        </>
      )}
    </div>
  );
};

export default ProductDetails;

