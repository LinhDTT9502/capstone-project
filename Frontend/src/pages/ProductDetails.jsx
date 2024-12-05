import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { fetchProductByProductCode } from "../services/productService";
import { Button, Input } from "@material-tailwind/react";
import { useTranslation } from "react-i18next";
import AddToCart from "../components/Product/AddToCart";
import RentalButton from "../components/Rental/RentalButton";
import { ProductColor } from "../components/Product/ProductColor";
import { ProductSize } from "../components/Product/ProductSize";
import { ProductCondition } from "../components/Product/ProductCondition";
import { HashLoader } from "react-spinners"
import { checkQuantityProduct } from "../services/warehouseService";
import { getComment } from "../services/Comment/CommentService";
import CommentList from "../components/Comment/CommentList";
// import CommentList from "../components/Comment/CommentList";

const ProductDetails = () => {
  const { productCode } = useParams();
  const [product, setProduct] = useState(null);
  const [displayImage, setDisplayImage] = useState(""); // Separate state for the image
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
  const [comments, setComments] = useState([])


  useEffect(() => {
    const getProduct = async () => {
      try {
        const productData = await fetchProductByProductCode(productCode, selectedColor, selectedSize, selectedCondition);
        if (productData.length > 0) {
          setProduct(productData[0]);
          setDisplayImage(productData[0].imgAvatarPath); // Set the initial image
        }
        setLoading(false);
      } catch (error) {
        setError(error);
        setLoading(false);
      }
    };

    getProduct();
  }, [productCode]);

  // Handle image change when only the color changes
  useEffect(() => {
    const fetchImage = async () => {
      try {
        const productData = await fetchProductByProductCode(productCode, selectedColor, "", "");
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
  }, [selectedColor]);

  // Handle product update when all fields are selected
  useEffect(() => {
    const fetchUpdatedProduct = async () => {
      try {
        const productData = await fetchProductByProductCode(productCode, selectedColor, selectedSize, selectedCondition);


        if (productData.length > 0) {
          setProduct(productData[0]);
        }
      } catch (error) {
        console.error("Error fetching updated product:", error);
      }
    };

    if (selectedColor && selectedSize && selectedCondition) {
      fetchUpdatedProduct();
      // console.log(product);
    }
  }, [selectedColor, selectedSize, selectedCondition]);

  // // Check if all fields are selected
  // useEffect(() => {
  //   if (!selectedColor || !selectedSize || !selectedCondition) {
  //     setIsFormValid(false);
  //     setWarning("Vui lòng chọn màu sắc, kích cỡ và tình trạng của sản phẩm");
  //   } else {
  //     setIsFormValid(true);
  //     setWarning("");
  //   }
  // }, [selectedColor, selectedSize, selectedCondition]);
  if (loading) {
    return <div className="flex justify-center items-center h-screen flex-col space-y-20">
      <HashLoader color="#ff6800" size={80} />
      <p>Đang tải chi tiết sản phẩm, vui lòng chờ một chút...</p>
    </div>

  }

  if (error) {
    return <div>{t("product_details.error")}</div>;
  }

  const handleRentalClick = () => {
    if (!selectedColor) {
      alert('Vui lòng chọn màu sắc, kích cỡ và tình trạng của sản phẩm!')
    } else if (!selectedSize) {
      alert('Vui lòng chọn kích cỡ và tình trạng của sản phẩm!')
    } else if (!selectedCondition) {
      alert('Vui lòng chọn tình trạng của sản phẩm!')
    } else {
      const rentalData = { product, quantity };
      localStorage.setItem("rentalData", JSON.stringify(rentalData));
      navigate("/rental-order");
      // console.log(localStorage.getItem("rentalData"));

    }

  };

  const handlePlaceOrder = async () => {
    if (!selectedColor) {
      alert('Vui lòng chọn màu sắc, kích cỡ và tình trạng của sản phẩm!')
    } else if (!selectedSize) {
      alert('Vui lòng chọn kích cỡ và tình trạng của sản phẩm!')
    } else if (!selectedCondition) {
      alert('Vui lòng chọn tình trạng của sản phẩm!')
    } else {
      const response = await checkQuantityProduct(product.id)
      if (quantity <= response.availableQuantity) {
        navigate("/sale-order", { state: { selectedProducts: product } });

      } else {
        alert(`Sản phẩm này chỉ còn lại ${response.availableQuantity} sản phẩm trong kho`)
      }
      // console.log(product);


    }
  };


  return (
    <div className="container mx-auto px-20 py-10 bg-white rounded-lg shadow-lg">
      {product && (
        <>
          <div className="flex flex-col  md:flex-row gap-1 justify-between">
            <div className="h-1/2 w-2/5">
              <img
                src={displayImage}
                alt={product.imgAvatarName || "Product Image"}
                className=" w-90 object-contain rounded-lg justify-center"
              />
              <div className="flex flex-wrap mt-4 gap-2">
                {product.listImages?.$values.map((image, index) => (
                  <img
                    key={index}
                    src={image}
                    alt={`Thumbnail ${index + 1}`}
                    onClick={() => setDisplayImage(image)}
                    className={`w-20 h-20 object-contain border-2 rounded-md cursor-pointer ${displayImage === image ? "border-orange-500" : "border-gray-300"
                      }`}
                  />
                ))}
              </div>
            </div>
            <div className="md:w-1/2">
              <h4 className="text-lg text-orange-500">{product.categoryName}</h4>
              <h2 className="text-3xl font-bold text-black mt-2">{product.productName}</h2>

              <div className="my-4 text-gray-800">
                <p><strong>Thương hiệu:</strong> {product.brandName}</p>
                <p><strong>Mã sản phẩm:</strong> {product.productCode}</p>
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
                <p><strong>Giá:</strong> {product.price ? `${product.price.toLocaleString()} ₫` : "N/A"}</p>
                <p><strong>Giá thuê:</strong> {product.rentPrice ? `${product.rentPrice.toLocaleString()} ₫` : "Sản phẩm chỉ bán"}</p>
                <div className="flex w-1/6 space-x-4">
                  <button
                    onClick={() => setQuantity(prev => prev - 1)}
                    className="px-3 bg-orange-500 text-white border rounded-md"
                  >
                    -
                  </button>
                  <Input
                    type="number"
                    value={quantity}
                    onChange={(e) => setQuantity(Math.max(1, Number(e.target.value)))}
                    className="w-24 h-6 text-center text-sm p-1 border rounded"
                    min="0"
                  />


                  <button
                    onClick={() => setQuantity(prev => prev + 1)}
                    className="px-3 border rounded-md bg-orange-500 text-white"
                  >
                    +
                  </button>
                </div>

              </div>
              <div className="space-y-3 w-full">
                <div className="flex items-center mt-4 space-x-2 w-full justify-between">
                  <Button
                    variant="text"
                    onClick={handlePlaceOrder}
                    className="w-full bg-orange-500 border-orange-500 border-2 text-white hover:text-orange-500 hover:bg-white">
                    Mua ngay
                  </Button>

                  <Button
                    variant="text"
                    onClick={handleRentalClick}
                    disabled={!product.isRent}
                    className="w-full bg-rose-700 border-rose-700 border-2 text-white hover:text-rose-700 hover:bg-white">
                    Thuê ngay
                  </Button>
                </div>
                <AddToCart
                  product={product}
                  quantity={quantity}
                  selectedColor={selectedColor}
                  selectedSize={selectedSize}
                  selectedCondition={selectedCondition} />
              </div>
              <div className="relative mt-6 border border-orange-400 rounded-lg p-4 bg-orange-50">
                <h3 className="absolute -mt-8 text-orange-600 bg-orange-100 px-2 font-semibold text-lg  border border-orange-500 rounded-lg flex items-center">
                  <span className="mr-2">🎁</span> ƯU ĐÃI
                </h3>
                <ul className="list-disc ml-5 space-y-2 text-gray-800">
                  <li>
                    Tặng 1 đôi vớ cầu lông (vớ <span className="text-orange-600 font-semibold">dài nhiều màu</span> hoặc <span className="text-orange-600 font-semibold">vớ ngắn</span>)
                  </li>
                  <li>Sản phẩm cam kết chính hãng</li>
                  <li>Thanh toán sau khi kiểm tra và nhận hàng</li>
                  <li>
                    Bảo hành chính hãng theo nhà sản xuất
                    <span className="text-gray-500">(Trừ hàng nội địa, xách tay)</span>
                  </li>
                </ul>

              </div>


            </div>
          </div>
          <div className="mt-4">
            <h3 className="font-poppins text-lg text-orange-500 font-bold">Mô tả sản phẩm:{product.description}</h3>
          </div>
        <CommentList
        productId={product?.id}
        />
        </>
      )}
    </div>
  );
};

export default ProductDetails;
