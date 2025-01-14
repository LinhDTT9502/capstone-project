import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { Link } from "react-router-dom";
import { fetchProducts } from "../services/productService";
import { selectProducts, setProducts } from "../redux/slices/productSlice";
import Paginationv3 from "../components/Product/Paginationv3";

const ProductList = ({
  sortBy,
  isAscending,
  selectedBrands,
  selectedCategories,
  minPrice,
  maxPrice,
}) => {
  const dispatch = useDispatch();
  const { products: allProducts } = useSelector(selectProducts) || {
    products: [],
  };
  const [filteredProducts, setFilteredProducts] = useState([]);

  const [currentPage, setCurrentPage] = useState(1);
  const productsPerPage = 15;

  const indexOfLastProduct = currentPage * productsPerPage;
  const indexOfFirstProduct = indexOfLastProduct - productsPerPage;
  const currentProducts = filteredProducts.slice(
    indexOfFirstProduct,
    indexOfLastProduct
  );

  const totalPages = Math.ceil(filteredProducts.length / productsPerPage);

  const handlePageChange = (pageNumber) => {
    setCurrentPage(pageNumber);
  };

  useEffect(() => {
    const getProducts = async () => {
      try {
        const productsData = await fetchProducts();
        const sortedProducts = [...productsData.products].sort((a, b) => a.price - b.price);

        dispatch(
          setProducts({
            data: {
              products: sortedProducts,
              total: productsData.total,
            },
          })
        );
      } catch (error) {
        console.error("Error fetching products:", error);
      }
    };

    getProducts();
  }, [dispatch]);

  useEffect(() => {
    const filterProducts = () => {
      let result = [...allProducts];

      if (selectedBrands.length > 0) {
        result = result.filter((product) =>
          selectedBrands.includes(product.brandId.toString())
        );
      }
      if (selectedCategories.length > 0) {
        result = result.filter((product) =>
          selectedCategories.includes(product.categoryID.toString())
        );
      }

      result = result.filter(
        (product) => product.price >= minPrice && product.price <= maxPrice
      );

      if (sortBy === "price") {
        result.sort((a, b) =>
          isAscending ? a.price - b.price : b.price - a.price
        );
      }

      setFilteredProducts(result);
    };

    filterProducts();
  }, [
    allProducts,
    sortBy,
    isAscending,
    selectedBrands,
    selectedCategories,
    minPrice,
    maxPrice,
  ]);

  return (
    <div className="container mx-auto">
      <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
        {currentProducts.length > 0
          ? currentProducts.map((product) => (
              <div
                key={product.id}
                className="bg-white border border-gray-200 hover:border-red-500 hover:drop-shadow-lg p-4 relative flex flex-col justify-between text-left h-full hover:cursor-pointer"
              >
                {/* Discount Badge */}
                {product.discount > 0 && (
                  <div className="absolute top-2 right-2 bg-orange-500 text-white text-xs font-bold px-2 py-1 rounded">
                    -{product.discount}%
                  </div>
                )}

                {/* Product Image */}
                <Link
                  to={`/product/${product.productCode}`}
                  className="flex-1 flex items-center justify-center"
                >
                  <img
                    src={product.imgAvatarPath}
                    alt={product.productName}
                    className="object-contain w-full max-h-48 mx-auto"
                  />
                </Link>

                {/* Product Name */}
                <div className="mt-4">
                  <h3
                    className="font-semibold text-lg line-clamp-2 overflow-hidden text-ellipsis"
                    style={{ maxHeight: "3rem" }} // Giới hạn chiều cao cho 2 dòng
                  >
                    {product.productName}
                  </h3>
                </div>

                <div className="h-px bg-gray-300 my-4 sm:my-2"></div>

                {/* Product Price */}
                <div>
                  {product.price !== product.listedPrice ? (
                    <>
                      <span className="text-red-700 text-lg font-bold relative group">
                        Giá bán: {product.price.toLocaleString("VI-vn")}₫{" "}
                        {/* Tooltip */}
                        <div className="absolute bottom-full left-1/2 transform -translate-x-1/2 mb-1 w-max bg-gray-500 text-white text-sm px-3 py-2 rounded hidden group-hover:block z-10 shadow-lg">
                          <p className="mb-1">
                            💰 <strong>Chi tiết giảm giá:</strong>
                          </p>
                          <ul className="list-disc list-inside">
                            <li>
                              Giá gốc:{" "}
                              {product.listedPrice.toLocaleString("VI-vn")}₫
                            </li>
                            <li>
                              Giảm giá:{" "}
                              {(
                                product.listedPrice - product.price
                              ).toLocaleString("VI-vn")}
                              ₫
                            </li>
                            <li>
                              Giá sau giảm:{" "}
                              {product.price.toLocaleString("VI-vn")}₫
                            </li>
                            <li>
                              Phần trăm giảm:{" "}
                              {(
                                ((product.listedPrice - product.price) /
                                  product.listedPrice) *
                                100
                              ).toFixed(0)}
                              %
                            </li>
                          </ul>
                        </div>
                      </span>{" "}
                      -{" "}
                      <span className="line-through text-gray-500 text-lg">
                        {product.listedPrice.toLocaleString("VI-vn")}₫
                      </span>
                    </>
                  ) : (
                    <span className="text-red-700 text-base font-bold">
                      Giá bán: {product.listedPrice.toLocaleString("VI-vn")}₫
                    </span>
                  )}
                  <br />
                  {/* Product rent price */}
                  {product.isRent ? (
                    <span className="text-blue-700 text-base font-bold">
                      Giá thuê: {product.rentPrice.toLocaleString("VI-vn")}₫
                    </span>
                  ) : (
                    <span className="text-gray-700 text-base font-bold">
                      Sản phẩm chỉ bán
                    </span>
                  )}
                </div>
              </div>
            ))
          : "Không có sản phẩm được tìm thấy"}
      </div>
      {/* Pagination */}
      <Paginationv3
        currentPage={currentPage}
        totalPages={totalPages}
        onPageChange={handlePageChange}
      />
    </div>
  );
};

export default ProductList;
