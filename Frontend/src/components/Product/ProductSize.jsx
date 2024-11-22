import { useState, useEffect } from "react";
import { fetchProductSize } from "../../services/productService";

export function ProductSize({ productCode, color, selectedSize, setSelectedSize }) {
  const [sizes, setSizes] = useState([]);

  useEffect(() => {
    const loadSizes = async () => {
      try {
        const data = await fetchProductSize(productCode, color);
        setSizes(data);
      } catch (error) {
        console.error("Failed to load product sizes:", error);
      }
    };

    if (productCode && color) {
      loadSizes();
    }
  }, [productCode, color]);

  const handleSizeChange = (size) => {
    setSelectedSize(size);
  };

  return (
    <div className="w-72">
      <p><strong>Size:</strong></p>
      <ul className="flex space-x-4">
        {sizes.map((size) => (
          <li key={size.size} className="flex items-center">
            <input
              type="radio"
              id={`size-${size.size}`}
              name="product-size"
              value={size.size}
              checked={selectedSize === size.size}
              onChange={() => handleSizeChange(size.size)}
              className="mr-2"
            />
            <label
              htmlFor={`size-${size.size}`}
              className={`cursor-pointer ${
                size.status ? "text-black" : "text-gray-400"
              }`}
            >
              {size.size} {size.status ? "" : "(Unavailable)"}
            </label>
          </li>
        ))}
      </ul>
      {/* {selectedSize && (
        <div className="mt-4 text-sm">
          Selected Size: <strong>{selectedSize}</strong>
        </div>
      )} */}
    </div>
  );
}