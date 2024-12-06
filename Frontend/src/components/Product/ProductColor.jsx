import { useState, useEffect } from "react";
import { fetchProductByProductCode, fetchProductColor } from "../../services/productService";

export function ProductColor({ productCode, selectedColor, setSelectedColor }) {
  const [colors, setColors] = useState([]);
  const [colorImages, setColorImages] = useState({});

  useEffect(() => {
    const loadColors = async () => {
      try {
        const data = await fetchProductColor(productCode);
        setColors(data);

        const imagePromises = data.map(async (color) => {
          const productData = await fetchProductByProductCode(productCode, color.color, "", "");
          return { color: color.color, image: productData[0]?.imgAvatarPath || "" };
        });

        const images = await Promise.all(imagePromises);
        const imagesMap = images.reduce((acc, { color, image }) => {
          acc[color] = image;
          return acc;
        }, {});

        setColorImages(imagesMap);

        if (!selectedColor && data.length > 0) {
          setSelectedColor(data[0].color);
        }
      } catch (error) {
        console.error("Failed to load product colors or images:", error);
      }
    };

    if (productCode) {
      loadColors();
    }
  }, [productCode, selectedColor, setSelectedColor]);

  return (
    <div className="space-y-2">
      <label className="text-gray-700 font-medium">Màu sắc:</label>
      <div className="flex flex-wrap gap-4">
        {colors.map((color, index) => (
          <label
            key={index}
            className={`flex items-center gap-2 p-2 rounded-lg border cursor-pointer transition-all ${
              selectedColor === color.color
                ? "border-orange-500 bg-orange-50"
                : "border-gray-200 hover:border-orange-200"
            }`}
          >
            <input
              type="radio"
              name="product-color"
              value={color.color}
              checked={selectedColor === color.color}
              onChange={() => setSelectedColor(color.color)}
              className="hidden"
            />
            <div className="relative w-5 h-5">
              <div className={`absolute inset-0 rounded-full border-2 ${
                selectedColor === color.color ? "border-orange-500" : "border-gray-300"
              }`} />
              {selectedColor === color.color && (
                <div className="absolute inset-1 rounded-full bg-orange-500" />
              )}
            </div>
            <span className="text-sm">{color.color}</span>
            {colorImages[color.color] && (
              <img
                src={colorImages[color.color]}
                alt={`Color option for ${color.color}`}
                className="w-10 h-10 object-cover rounded-md"
              />
            )}
          </label>
        ))}
      </div>
    </div>
  );
}
