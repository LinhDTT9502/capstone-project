import React, { useState, useRef, useEffect } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMagnifyingGlass } from "@fortawesome/free-solid-svg-icons";
import { searchProducts } from "../../api/apiProduct";
import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";

const SearchBar = () => {
  const [query, setQuery] = useState("");
  const [results, setResults] = useState([]);
  const searchRef = useRef(null);
  const { t } = useTranslation();

  // Handle search input
  const handleSearch = async (e) => {
    const value = e.target.value;
    setQuery(value);
    if (value.trim()) {
      try {
        const response = await searchProducts(value);
        setResults(response.data.data?.$values || []);
      } catch (error) {
        console.error("Error searching products:", error);
        setResults([]);
      }
    } else {
      setResults([]);
    }
  };

  // Handle click outside
  useEffect(() => {
    const handleClickOutside = (e) => {
      if (searchRef.current && !searchRef.current.contains(e.target)) {
        setResults([]);
      }
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  return (
    <div className="relative z-50 w-full sm:w-1/3 p-2 mx-auto" ref={searchRef}>
      {/* Search Input */}
      <div className="flex w-full bg-white border-2 border-orange-500 rounded-full px-4 py-2 items-center shadow-sm">
        <input
          className="flex-grow bg-transparent outline-none placeholder-gray-400"
          placeholder={t("search_bar.search_placeholder") || "Tìm kiếm sản phẩm..."}
          type="text"
          value={query}
          onChange={handleSearch}
        />
        <FontAwesomeIcon
          icon={faMagnifyingGlass}
          className="text-orange-500 font-medium cursor-pointer"
        />
      </div>

      {/* Search Results */}
      {results.length > 0 && (
        <div className="absolute z-40 w-full bg-white border border-gray-300 rounded-lg shadow-lg mt-2 overflow-hidden max-h-60 overflow-y-auto">
          {results.map((product) => (
            <Link
              to={`/product/${product.productCode}`}
              className="block p-3 hover:bg-gray-100 transition-colors"
            >
              <div className="flex items-center space-x-4">
                {/* Product Image with fallback */}
                <img
                  src={product.imgAvatarPath || "/assets/default-product.png"}
                  alt={product.productName || "Product"}
                  className="w-12 h-12 object-cover rounded"
                  onError={(e) => (e.target.src = "/assets/default-product.png")}
                />
                <span className="text-gray-800 font-medium">
                  {product.productName}
                </span>
              </div>
            </Link>
          ))}
        </div>
      )}
    </div>
  );
};

export default SearchBar;
