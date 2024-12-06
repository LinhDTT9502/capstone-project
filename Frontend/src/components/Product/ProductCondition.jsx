import { useState, useEffect } from "react";
import { fetchProductCondition } from "../../services/productService";

export function ProductCondition({ productCode, color, size, selectedCondition, setSelectedCondition }) {
  const [conditions, setConditions] = useState([]);

  useEffect(() => {
    const loadConditions = async () => {
      try {
        const response = await fetchProductCondition(productCode, color, size);
        setConditions(response);

        if (!selectedCondition && response.length > 0) {
          const availableCondition = response.find(cond => cond.status);
          if (availableCondition) {
            setSelectedCondition(availableCondition.condition);
          }
        }
      } catch (error) {
        console.error("Failed to load product conditions:", error);
      }
    };

    if (productCode && color && size) {
      loadConditions();
    }
  }, [productCode, color, size, selectedCondition, setSelectedCondition]);

  return (
    <div className="space-y-2">
      <label className="text-gray-700 font-medium">Tình trạng:</label>
      <div className="flex flex-wrap gap-2">
        {conditions.map((condition) => (
          <label
            key={condition.condition}
            className={`flex items-center justify-center px-4 py-2 rounded-lg border cursor-pointer transition-all ${
              selectedCondition === condition.condition
                ? "border-orange-500 bg-orange-50 text-orange-500"
                : condition.status
                ? "border-gray-200 hover:border-orange-200"
                : "border-gray-200 bg-gray-100 text-gray-400 cursor-not-allowed"
            }`}
          >
            <input
              type="radio"
              name="product-condition"
              value={condition.condition}
              checked={selectedCondition === condition.condition}
              onChange={() => setSelectedCondition(condition.condition)}
              className="hidden"
              disabled={!condition.status}
            />
            <span className="text-sm font-medium">{condition.condition}%</span>
          </label>
        ))}
      </div>
    </div>
  );
}

