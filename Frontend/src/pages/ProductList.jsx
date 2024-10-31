import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { Link } from 'react-router-dom';
import { fetchProducts, fetchProductsFiltered } from '../services/productService';
import { addToCart } from '../services/cartService';
import { selectProducts, setProducts } from '../redux/slices/productSlice';
import { toast } from "react-toastify";
import { addCart } from '../redux/slices/cartSlice';
import { Rating } from "@material-tailwind/react";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faChevronLeft, faChevronRight, faCartShopping } from '@fortawesome/free-solid-svg-icons';

import { useTranslation } from "react-i18next";
import { fetchWarehouse } from '../services/warehouseService';
const perPage = 15;

const ProductList = ({ sortBy, isAscending, selectedBrands, selectedCategories, minPrice, maxPrice }) => {
  const { t } = useTranslation();
  const dispatch = useDispatch();
  const { products } = useSelector(selectProducts) || { products: [] };
  const [quantity, setQuantity] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [warehouseTotal, setWarehouseTotal] = useState(null);
  const totalPages = Math.ceil(warehouseTotal / perPage);

  useEffect(() => {
    const getProducts = async () => {
      try {
        let productsData;
        if (
          sortBy === "" &&
          selectedBrands.length === 0 &&
          selectedCategories.length === 0 &&
          minPrice === 0 &&
          maxPrice === 3000000
        ) {
          productsData = await fetchProducts(currentPage);
        } else {
          productsData = await fetchProductsFiltered(
            sortBy,
            isAscending,
            selectedBrands,
            selectedCategories,
            minPrice,
            maxPrice
          );
        }
        await checkWarehouseQuantities(productsData.products);
        dispatch(setProducts({ data: { products: productsData.products, total: productsData.total } }));
        console.log(productsData.products);
        
      } catch (error) {
        console.error('Error fetching products:', error);
      }
    };

    // const fetchWarehouseData = async () => {
    //   const response = await fetch('https://twosportapi-295683427295.asia-southeast2.run.app/api/Warehouse/list-all', {
    //     headers: {
    //       'accept': '*/*'
    //     }
    //   });
    //   const data = await response.json();
    //   return { total: data.total, products: data.data.$values };
    // };

    const checkWarehouseQuantities = async (products) => {
      try {
          const warehouseData = await fetchWarehouse();
          console.log(warehouseData);
          
          setWarehouseTotal(warehouseData.total);
          products.forEach(product => {
              const warehouseProduct = warehouseData.products.find(w => w.productName === product.productName);
              if (warehouseProduct) {
                  product.warehouseId = warehouseProduct.id;
                  product.quantity = warehouseProduct.quantity;
              }
          });
      } catch (error) {
          console.error('Error checking warehouse quantities:', error);
      }
  };
  getProducts();
  }, [currentPage, sortBy, isAscending, minPrice, maxPrice, selectedCategories, selectedBrands, dispatch]);

  const handleAddToCart = async (product, quantityToAdd = 1) => {
    const token = localStorage.getItem('token');
    if (!token) {
      // If no token is found, notify the user
      dispatch(addCart(product));
      toast.info('Added to cart');
      return;
    }

    // Find the warehouse associated with the product
    const warehouseProduct = products.find(p => p.productName === product.productName);
    if (!warehouseProduct || !warehouseProduct.warehouseId) {
      toast.error('Warehouse data unavailable for this product.');
      return;
    }

    const warehouseId = warehouseProduct.warehouseId; // Get the warehouseId from the matched warehouse product
    const newQuantity = quantity + quantityToAdd; // Increase the quantity by the amount to add

    try {
      // Send warehouseId and quantity to the API
      await addToCart(warehouseId, newQuantity, token);

      setQuantity(newQuantity);
      toast.success(`${product.productName} has been added to the cart!`);
    } catch (error) {
      console.error(`${t("product_list.error_adding_product_to_cart")}:`, error);
      toast.error(`${t("product_list.error_adding_product_to_cart")}`);
    }
};


  const handlePrevPage = () => {
    if (currentPage > 1) setCurrentPage(currentPage - 1);
  };

  const handleNextPage = () => {
    if (currentPage < totalPages) setCurrentPage(currentPage + 1);
    console.log(currentPage);
  };

  return (
    <div className="">
      <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
        {products?.map(product => (
          <div key={product.id} className="bg-white border hover:drop-shadow-lg  hover:bg-blue-100 p-2">
            <div className="relative">
              <Link to={`/product/${product.id}`}>
                <div className="bg-white">
                  <img src={product.imgAvatarPath} alt={product.mainImageName} className="object-scale-down h-48 w-96 mb-4" />
                </div>
              </Link>
              {product.quantity === 0 ? (
                <button
                  className="absolute bottom-0 left-0 right-0 flex items-center justify-center bg-gray-400 bg-opacity-75 text-white opacity-100 transition-opacity duration-300 py-4"
                  disabled
                >
                  SOLD OUT
                </button>
              ) : (
                <button
                  className="absolute bottom-0 left-0 right-0 flex items-center justify-center bg-orange-600 bg-opacity-75 text-white opacity-0 hover:opacity-100 transition-opacity duration-300 py-4"
                  onClick={() => handleAddToCart(product)}
                >
                  {t("product_list.add_to_cart")}
                </button>
              )}
            </div>
            <div className='flex justify-between space-x-4'>
              <button
                className="p-3 bg-blue-300 rounded-xl items-center justify-center bg-opacity-75 text-black opacity-100 hover:bg-blue-500 transition duration-300 py-4 hover:shadow-lg"
                onClick={() => handleAddToCart(product)}
              >
                Mua ngay
              </button>
              <button
                className="p-3 bg-orange-300 rounded-xl  items-center justify-center bg-opacity-75 text-black opacity-100 hover:bg-orange-500 transition duration-300 py-4 hover:shadow-lg"
                onClick={() => handleAddToCart(product)}
              >
                Thuê
              </button>
            </div>

            <Link to={`/product/${product.id}`}>
              <p className="text-orange-500 mb-2 ">{product.categoryName} - {product.brandName}</p>
              <h2 className="text-xl font-semibold ">{product.productName}</h2>
              <p className="text-gray-700 mb-2">{product.price.toLocaleString()} {t("product_list.vnd")}</p>
            </Link>
          </div>
        ))}
      </div>
      <button
        disabled={currentPage === 1}
        onClick={handlePrevPage}
      >
        <FontAwesomeIcon icon={faChevronLeft} />
      </button>
      <span className="px-4 py-2">{currentPage}</span>
      <button
        disabled={currentPage === totalPages}
        onClick={handleNextPage}
      >
        <FontAwesomeIcon icon={faChevronRight} />
      </button>
    </div>
  );
};

export default ProductList;