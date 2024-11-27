import { useState, useEffect } from "react";
import { fetchProductByProductCode, fetchProductColor } from "../../services/productService";

export function ProductColor({ productCode, selectedColor, setSelectedColor }) {
  const [colors, setColors] = useState([]);
  const [colorImages, setColorImages] = useState({}); // Store images for each color

  useEffect(() => {
    const loadColors = async () => {
      try {
        const data = await fetchProductColor(productCode);
        setColors(data);
        // if (data.length > 0) {
        //   setSelectedColor(data[0].color);
        // }

        // Fetch images for each color
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
      } catch (error) {
        console.error("Failed to load product colors or images:", error);
      }
    };

    if (productCode) {
      loadColors();
    }
  }, [productCode]);

  return (
    <div className="w-full">
      <p><strong>Màu sắc:</strong></p>
      <div className="flex flex-wrap gap-4">
        {colors.map((color, index) => (
          <label
            key={index}
            className="flex items-center gap-2 cursor-pointer p-2 border rounded-md"
          >
            <input
              type="radio"
              name="product-color"
              value={color.color}
              checked={selectedColor === color.color}
              onChange={() => setSelectedColor(color.color)}
              className="w-4 h-4 accent-blue-500"
            />
            <span>{color.color}</span>
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
