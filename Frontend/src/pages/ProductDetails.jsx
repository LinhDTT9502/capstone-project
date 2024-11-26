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
import {HashLoader} from "react-spinners"

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
    }
  }, [selectedColor, selectedSize, selectedCondition]);
  
  // Check if all fields are selected
  useEffect(() => {
    if (!selectedColor || !selectedSize || !selectedCondition) {
      setIsFormValid(false);
      setWarning("Vui lòng chọn màu sắc, kích cỡ và tình trạng của sản phẩm");
    } else {
      setIsFormValid(true);
      setWarning("");
    }
  }, [selectedColor, selectedSize, selectedCondition]);
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
    const rentalData = { product, quantity }; 
    localStorage.setItem("rentalData", JSON.stringify(rentalData));
    navigate("/rental-order");
  };

  return (
    <div className="container mx-auto px-20 py-6 bg-white rounded-lg shadow-lg">
      {product && (
        <div className="flex flex-col justify-center items-center md:flex-row gap-1">
          <div className="md:w-1/2 flex justify-center items-center">
            <img
              src={displayImage}
              alt={product.imgAvatarName || "Product Image"}
              className="w-1/2 h-auto object-contain rounded-lg"
            />
          </div>
          <div className="md:w-1/2">
            <h4 className="text-lg text-orange-500">{product.categoryName}</h4>
            <h2 className="text-3xl font-bold text-black mt-2">{product.productName}</h2>

            <div className="my-4 text-gray-800">
              <p><strong>Brand:</strong> {product.brandName}</p>
              <p><strong>Code:</strong> {product.productCode}</p>
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
              <p><strong>Price:</strong> {product.price ? `${product.price.toLocaleString()} VND` : "N/A"}</p>
              <p><strong>Rent Price:</strong> {product.rentPrice ? `${product.rentPrice.toLocaleString()} VND` : "N/A"}</p>
              <div className="flex w-1/6 space-x-4">
              <button
                onClick={() => setQuantity(prev => prev - 1)}
                className="px-3 py-1 border rounded-md"
              >
                -
              </button>
              <Input
                type="number"
                value={quantity}
                onChange={(e) => setQuantity(Math.max(1, Number(e.target.value)))}
                className="w-full text-center"
                min="1"
              />
              <button
                onClick={() => setQuantity(prev => prev + 1)}
                className="px-3 py-1 border rounded-md"
              >
                +
              </button>
              </div>
             
            </div>

            <div className="flex items-center mt-4 space-x-2">
            {warning && <div className="text-red-500 text-sm">{warning}</div>}

              <AddToCart product={product} quantity={quantity} isFormValid={isFormValid}/>
              {/* <RentalButton /> */}
              <Button color="orange" onClick={handleRentalClick} disabled={!isFormValid}>
                Thuê ngay
              </Button>

            </div>
          </div>

        </div>
      )}
    </div>
  );
};

export default ProductDetails;
