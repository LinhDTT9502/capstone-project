import React, { useState, useEffect } from 'react';
import './PriceRangeSlider.css';

const PriceRangeSlider = ({ minPrice, maxPrice, setMinPrice, setMaxPrice }) => {
  const min = 0;
  const max = 100000000;

  const [minThumb, setMinThumb] = useState(0);
  const [maxThumb, setMaxThumb] = useState(0);

  useEffect(() => {
    minTrigger();
    maxTrigger();
  }, [minPrice, maxPrice]);

  const minTrigger = () => {
    const newMinPrice = Math.min(minPrice, maxPrice - 1000);
    setMinPrice(newMinPrice);
    setMinThumb(((newMinPrice - min) / (max - min)) * 100);
  };

  const maxTrigger = () => {
    const newMaxPrice = Math.max(maxPrice, minPrice + 1000);
    setMaxPrice(newMaxPrice);
    setMaxThumb(100 - (((newMaxPrice - min) / (max - min)) * 100));
  };

  const handleMinChange = (e) => {
    const value = Number(e.target.value);
    // Prevent value less than 0
    if (value >= 0 && value <= maxPrice - 1000) {
      setMinPrice(value);
    }
  };

  const handleMaxChange = (e) => {
    const value = Number(e.target.value);
    // Prevent value greater than max and less than minPrice + 1000
    if (value <= max && value >= minPrice + 1000) {
      setMaxPrice(value);
    }
  };

  return (
    <div className="pt-10 flex justify-center items-center">
      <div className="relative max-w-xl w-full">
        <div>
          <input
            type="range"
            step="100"
            min={min}
            max={max}
            value={minPrice}
            onChange={handleMinChange}
            className="absolute pointer-events-none appearance-none z-20 h-2 w-full opacity-0 cursor-pointer"
          />
          <input
            type="range"
            step="100"
            min={min}
            max={max}
            value={maxPrice}
            onChange={handleMaxChange}
            className="absolute pointer-events-none appearance-none z-20 h-2 w-full opacity-0 cursor-pointer"
          />
          <div className="relative z-10 h-1">
            <div className="absolute z-10 left-0 right-0 bottom-0 top-0 rounded-md bg-gray-200"></div>
            <div
              className="absolute z-20 top-0 bottom-0 rounded-md bg-orange-500"
              style={{ right: `${maxThumb}%`, left: `${minThumb}%` }}
            ></div>
            <div
              className="absolute z-30 w-5 h-5 top-0 bg-orange-500 border-rose-700 border-2 rounded-full -mt-2 -ml-1"
              style={{ left: `${minThumb}%` }}
            ></div>
            <div
              className="absolute z-30 w-5 h-5 top-0 bg-orange-500 rounded-full border-rose-700 border-2 -mt-2 -mr-3"
              style={{ right: `${maxThumb}%` }}
            ></div>
          </div>
        </div>
        <div className="flex justify-between items-center space-x-5 py-5">
          <div className="flex py-2 px-2 rounded border justify-between w-1/2">
            <input
              type="number"
              maxLength="9"
              value={minPrice}
              onChange={handleMinChange}
              className="w-2/3 text-center"
              min="0" // Adding the min attribute for the input
            />
            ₫
          </div>
          <div className="flex py-2 px-2 rounded border justify-between w-1/2">
            <input
              type="number"
              maxLength="9"
              value={maxPrice}
              onChange={handleMaxChange}
              className="w-2/3 text-center"
              max={max} // Adding the max attribute for the input
            />
            ₫
          </div>
        </div>
      </div>
    </div>
  );
};

export default PriceRangeSlider;
