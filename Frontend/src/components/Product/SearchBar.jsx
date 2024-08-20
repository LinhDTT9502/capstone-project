import React, { useState } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faMagnifyingGlass } from '@fortawesome/free-solid-svg-icons';
import { searchProducts } from '../../api/apiProduct';
import { Link } from 'react-router-dom';
import { useTranslation } from "react-i18next";

const SearchBar = () => {
  const [query, setQuery] = useState('');
  const [results, setResults] = useState([]);
  const { t } = useTranslation();

  const handleSearch = async (e) => {
    setQuery(e.target.value);
    if (e.target.value) {
      try {
        const response = await searchProducts(e.target.value);
        setResults(response.data.data.$values);
      } catch (error) {
        console.error('Error searching products:', error);
      }
    } else {
      setResults([]);
    }
  };

  return (
    <div className="relative z-50 w-1/4 p-2 mx-auto ">
      <div className="flex w-full bg-white border-2 border-orange-500 rounded-full p-2 mx-auto">
        <input
          className="flex-grow bg-transparent outline-none placeholder-gray-400"
          placeholder={t("search_bar.search_placeholder")}
          type="text"
          value={query}
          onChange={handleSearch}
        />
        <FontAwesomeIcon icon={faMagnifyingGlass} className="items-center text-orange-500 font-medium pr-3" />
      </div>
      {results.length > 0 && (
        <div className="absolute z-60 w-full bg-white border border-gray-300 rounded-lg shadow-lg mt-2">
          {results.map((product) => (
            <Link to={`/product/${product.id}`} key={product.id} className="block p-2 hover:bg-gray-200">
              <div className="flex items-center space-x-4">
                <img src={product.mainImagePath} alt={product.productName} className="w-12 h-12 object-cover" />
                <span>{product.productName}</span>
              </div>
            </Link>
          ))}
        </div>

      )}
    </div>
  );
};

export default SearchBar;
