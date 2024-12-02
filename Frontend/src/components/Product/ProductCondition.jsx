import { useState, useEffect } from "react";
import { fetchProductCondition } from "../../services/productService";

export function ProductCondition({ productCode, color, size, selectedCondition, setSelectedCondition }) {
  const [conditions, setConditions] = useState([]);

  useEffect(() => {
    const loadConditions = async () => {
      try {
        const response = await fetchProductCondition(productCode, color, size);
        setConditions(response);
        // if (response.length > 0) {
        //   setSelectedCondition(response[0].condition);
        // }
      } catch (error) {
        console.error("Failed to load product conditions:", error);
      }
    };

    if (productCode && color && size) {
      loadConditions();
    }
  }, [productCode, color, size]);

  const handleConditionChange = (condition) => {
    setSelectedCondition(condition);
  };

  return (
    <div className="w-full">
      <ul className="flex space-x-4 items-center">
     <p><strong>Tình trạng:</strong></p>
        {conditions.map((condition) => (
          <li key={condition.condition} className="flex items-center border-2 border-zinc-400 p-1">
            <input
              type="radio"
              id={`condition-${condition.condition}`}
              name="product-condition"
              value={condition.condition}
              checked={selectedCondition === condition.condition}
              onChange={() => handleConditionChange(condition.condition)}
              className="mr-1 "
              disabled={!condition.status}
            />
            <label
              htmlFor={`condition-${condition.condition}`}
              className={`cursor-pointer ${
                condition.status ? "text-black" : "text-gray-400"
              }`}
            >
              {condition.condition}% {condition.status ? "" : "(Unavailable)"}
            </label>
          </li>
        ))}
      </ul>
      {/* {selectedCondition && (
        <div className="mt-4 text-sm">
          Selected Condition: <strong>{selectedCondition}</strong>
        </div>
      )} */}
    </div>
  );
}
