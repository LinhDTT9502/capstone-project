import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { Link } from 'react-router-dom';
import { fetchProducts } from '../services/productService';
import { selectProducts, setProducts } from '../redux/slices/productSlice';
import { Rating } from "@material-tailwind/react";

const ProductList = ({ sortBy, isAscending, selectedBrands, selectedCategories, minPrice, maxPrice }) => {
  // console.log(selectedBrands,selectedCategories);
  
  const dispatch = useDispatch();
  const { products: allProducts } = useSelector(selectProducts) || { products: [] };
  const [filteredProducts, setFilteredProducts] = useState([]);

  useEffect(() => {
    const getProducts = async () => {
      try {
        const productsData = await fetchProducts();
        dispatch(setProducts({ data: { products: productsData.products, total: productsData.total } }));
      } catch (error) {
        console.error('Error fetching products:', error);
      }
    };

    getProducts();
  }, [dispatch]);

  useEffect(() => {
    // Filtering products based on the criteria
    const filterProducts = () => {
      let result = [...allProducts];

      if (selectedBrands.length > 0) {
        result = result.filter(product =>
          selectedBrands.includes(product.brandId.toString())
        );
      }
      if (selectedCategories.length > 0) {
        result = result.filter(product =>
          selectedCategories.includes(product.categoryID.toString())
        );
      }
      

      // Filter by price range
      result = result.filter(product => product.price >= minPrice && product.price <= maxPrice);

      // Sort the filtered results
      if (sortBy === 'price') {
        result.sort((a, b) => (isAscending ? a.price - b.price : b.price - a.price));
      }

      setFilteredProducts(result);
      // console.log(result);
      //  dispatch(setProducts({ data: { total: result.length } }));
      
    };

    filterProducts();
  }, [allProducts, sortBy, isAscending, selectedBrands, selectedCategories, minPrice, maxPrice]);

  return (
    <div className="">
      <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
        {filteredProducts.map(product => (
          <div key={product.id} className="bg-white border hover:drop-shadow-lg hover:bg-blue-100 p-2">
            <div className="relative">
              <Link to={`/product/${product.productCode}`}>
                <div className="bg-white">
                  <img src={product.imgAvatarPath} alt={product.productName} className="object-contain  w-96 h-48" />
                </div>
              </Link>
              <div className=" mt-2">
                <h3 className="font-semibold">{product.productName}</h3>
                <p className='text-red-700'>{product.price.toLocaleString()} ₫</p>
              
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default ProductList;
