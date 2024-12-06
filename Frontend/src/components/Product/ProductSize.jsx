import { useState, useEffect } from "react";
import { fetchProductSize } from "../../services/productService";

export function ProductSize({ productCode, color, selectedSize, setSelectedSize }) {
  const [sizes, setSizes] = useState([]);

  useEffect(() => {
    const loadSizes = async () => {
      try {
        const data = await fetchProductSize(productCode, color);
        setSizes(data);

        if (!selectedSize && data.length > 0) {
          const availableSize = data.find(size => size.status);
          if (availableSize) {
            setSelectedSize(availableSize.size);
          }
        }
      } catch (error) {
        console.error("Failed to load product sizes:", error);
      }
    };

    if (productCode && color) {
      loadSizes();
    }
  }, [productCode, color, selectedSize, setSelectedSize]);

  return (
    <div className="space-y-2">
      <label className="text-gray-700 font-medium">Kích cỡ:</label>
      <div className="flex flex-wrap gap-2">
        {sizes.map((size) => (
          <label
            key={size.size}
            className={`flex items-center justify-center w-12 h-12 rounded-lg border cursor-pointer transition-all ${
              selectedSize === size.size
                ? "border-orange-500 bg-orange-50 text-orange-500"
                : size.status
                ? "border-gray-200 hover:border-orange-200"
                : "border-gray-200 bg-gray-100 text-gray-400 cursor-not-allowed"
            }`}
          >
            <input
              type="radio"
              name="product-size"
              value={size.size}
              checked={selectedSize === size.size}
              onChange={() => setSelectedSize(size.size)}
              className="hidden"
              disabled={!size.status}
            />
            <span className="text-sm font-medium">{size.size}</span>
          </label>
        ))}
      </div>
    </div>
  );
}
