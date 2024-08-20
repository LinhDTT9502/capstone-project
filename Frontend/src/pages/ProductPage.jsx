import { useState, useEffect } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faTableCellsLarge,
  faBars,
  faXmark,
  faArrowUpWideShort,
  faArrowDownWideShort,
} from "@fortawesome/free-solid-svg-icons";
import PriceRangeSlider from "../components/Product/PriceRangeSlider ";
import ProductList from "./ProductList";
import { fetchBrands } from "../services/brandService";
import { fetchCategories } from "../services/categoryService";
import { Link } from "react-router-dom";
import { useSelector } from "react-redux";
import { selectProducts } from "../redux/slices/productSlice";
import { useTranslation } from "react-i18next";

function ProductPage() {
  const { t } = useTranslation();
  const [brands, setBrands] = useState([]);
  const [categories, setCategories] = useState([]);
  const [sortBy, setSortBy] = useState("");
  const [isAscending, setIsAscending] = useState(true);
  const [selectedBrands, setSelectedBrands] = useState([]);
  const [selectedCategories, setSelectedCategories] = useState([]);
  const [minPrice, setMinPrice] = useState(0);
  const [maxPrice, setMaxPrice] = useState(3000000);
  const products = useSelector(selectProducts);

  useEffect(() => {
    const getBrands = async () => {
      try {
        const brandsData = await fetchBrands();
        const filteredBrands = brandsData.filter((brand) => brand.quantity > 0);
        setBrands(filteredBrands);
      } catch (error) {
        console.error("Error fetching brand data:", error);
      }
    };

    getBrands();
  }, []);

  useEffect(() => {
    const getCategories = async () => {
      try {
        const categoriesData = await fetchCategories();
        const filteredCategories = categoriesData.filter(
          (category) => category.quantity > 0
        );
        setCategories(filteredCategories);
      } catch (error) {
        console.error("Error fetching category data:", error);
      }
    };

    getCategories();
  }, []);

  const handleSortChange = (e) => {
    setSortBy(e.target.value);
  };

  const handleAsc = () => {
    setIsAscending((prevState) => !prevState);
  };

  const handleClearFilters = () => {
    setSelectedBrands([]);
    setSelectedCategories([]);
    setMinPrice(0);
    setMaxPrice(3000000);
  };

  const handleBrandChange = (e) => {
    const value = e.target.value;
    setSelectedBrands((prevSelectedBrands) =>
      prevSelectedBrands.includes(value)
        ? prevSelectedBrands.filter((brand) => brand !== value)
        : [...prevSelectedBrands, value]
    );
  };

  const handleCategoryChange = (e) => {
    const value = e.target.value;
    setSelectedCategories((prevSelectedCategories) =>
      prevSelectedCategories.includes(value)
        ? prevSelectedCategories.filter((category) => category !== value)
        : [...prevSelectedCategories, value]
    );
  };

  return (
    <div className="pt-10 bg-zinc-50">
      <div className="w-full px-20">
        <div className="flex justify-between items-center"></div>
        <div className="grid grid-cols-1 lg:grid-cols-4 gap-4 mt-2">
          <div className="w-full lg:col-span-1">
            <div className="w-full">
              <div className="mb-4 font-alfa text-xl">
                {t("productv2.products")}
              </div>
              <div className="Products text-black font-bold">
                {t("productv2.categories")}
              </div>
              <div className="relative p-4">
                <div className="grid grid-cols-1 gap-2">
                  {categories.map((category, index) => (
                    <label key={index} className="inline-flex items-center">
                      <input
                        type="checkbox"
                        className="form-checkbox h-5 w-5 text-orange-500"
                        value={category.id}
                        onChange={handleCategoryChange}
                        checked={selectedCategories.includes(
                          category.id.toString()
                        )}
                      />
                      <span className="ml-2 text-black">
                        {category.categoryName} ({category.quantity})
                      </span>
                    </label>
                  ))}
                </div>
              </div>
              <div className="h-px bg-gray-300 my-5 mx-auto"></div>
              <div className="text-black font-bold">
                {t("productv2.brands")}
              </div>
              <div className="relative p-4">
                <div className="grid grid-cols-1 gap-2">
                  {brands.map((brand, index) => (
                    <label key={index} className="inline-flex items-center">
                      <input
                        type="checkbox"
                        className="form-checkbox h-5 w-5 text-orange-500"
                        value={brand.id}
                        onChange={handleBrandChange}
                        checked={selectedBrands.includes(brand.id.toString())}
                      />
                      <span className="ml-2 text-black">
                        {brand.brandName} ({brand.quantity})
                      </span>
                    </label>
                  ))}
                </div>
              </div>
              <div className="h-px bg-gray-300 my-5 mx-auto"></div>
              <div>
                <div className="text-black font-bold">
                  {t("productv2.price")}
                </div>
                <PriceRangeSlider
                  minPrice={minPrice}
                  maxPrice={maxPrice}
                  setMinPrice={setMinPrice}
                  setMaxPrice={setMaxPrice}
                />
                <div className="h-px bg-gray-300 my-5 mx-auto"></div>
                <div className="flex items-center justify-center mt-4 w-fit">
                  <button
                    onClick={handleClearFilters}
                    className="flex items-center text-black font-bold underline"
                  >
                    <FontAwesomeIcon icon={faXmark} />
                    {t("productv2.clear_all_filter")}
                  </button>
                </div>
              </div>
              <div className="relative inline-block mt-6">
                <img src="/assets/images/product/hero.png" alt="Hero" />
                <div className="absolute top-[10%] left-1/2 transform -translate-x-1/2 text-center p-[10px]">
                  <span className="text-white font-bold text-xl font-poppins">
                    {t("productv2.get_yours")}
                  </span>
                  <br />
                  <span className="text-white font-bold text-3xl uppercase font-poppins">
                    {t("productv2.best_gear")}
                  </span>
                  <br />
                </div>
                <div className="absolute top-[85%] left-1/2 transform -translate-x-1/2 -translate-y-1/2 bg-orange-500 p-2">
                  <span className="text-black font-bold">
                    {t("productv2.shop_now")}
                  </span>
                </div>
              </div>
            </div>
          </div>
          <div className="w-full lg:col-span-3">
            <div className="py-6">
              <div className="flex justify-between items-center border-b pb-4 mb-4">
                <div className="text-sm text-gray-600">
                  {t("productv2.showing")} {products.total}{" "}
                  {t("productv2.results")}
                </div>
                <div className="flex items-center">
                  <span className="mr-2 text-sm text-gray-600">
                    {t("productv2.sort_by")}
                  </span>
                  <select
                    className="block pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md"
                    value={sortBy}
                    onChange={handleSortChange}
                  >
                    <option value="">None</option>
                    <option value="price">Giá tiền</option>
                  </select>
                  {sortBy ? (
                    <button onClick={handleAsc}>
                      <FontAwesomeIcon
                        icon={
                          isAscending
                            ? faArrowUpWideShort
                            : faArrowDownWideShort
                        }
                      />
                    </button>
                  ) : (
                    ""
                  )}
                  <div className="ml-4 flex items-center space-x-2">
                    {/* <button>
                      <FontAwesomeIcon icon={faTableCellsLarge} />
                    </button> */}
                    {/* <Link to="/productv2">
                      <button>
                        <FontAwesomeIcon icon={faBars} />
                      </button>
                    </Link> */}
                  </div>
                </div>
              </div>
            </div>
            <div className="pl-10">
              <ProductList
                sortBy={sortBy}
                isAscending={isAscending}
                selectedBrands={selectedBrands}
                selectedCategories={selectedCategories}
                minPrice={minPrice}
                maxPrice={maxPrice}
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default ProductPage;
