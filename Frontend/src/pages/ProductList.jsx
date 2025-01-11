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

        dispatch(
          setProducts({
            data: {
              products: productsData.products,
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
                <div>
                  {/* Discount Badge */}
                  {product.discount > 0 && (
                    <div className="absolute top-2 right-2 bg-orange-500 text-white text-xs font-bold px-2 py-1 rounded">
                      -{product.discount}%
                    </div>
                  )}

                  {/* Product Image */}
                  <Link to={`/product/${product.productCode}`}>
                    <img
                      src={product.imgAvatarPath}
                      alt={product.productName}
                      className="object-contain w-full h-48 mx-auto"
                    />
                  </Link>

                  {/* Product Name */}
                  <h3 className="font-semibold mt-4">{product.productName}</h3>

                  {/* Product Price */}
                  {product.price !== product.listedPrice ? (
                    <>
                      <span className="text-red-700 text-lg font-bold mt-autrelative group">
                        {product.price.toLocaleString("VI-vn")}₫ {/* Tooltip */}
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
                    <span className="text-red-700 text-lg font-bold mt-aut">
                      {product.listedPrice.toLocaleString("VI-vn")}₫
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
