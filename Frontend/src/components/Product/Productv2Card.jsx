import { useState } from "react";

import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faHeart, faShareNodes } from "@fortawesome/free-solid-svg-icons";
import {Rating } from "@material-tailwind/react";
import Pagination from "../Product/Paginationv2";

const products = [
  {
    id: 1,
    image: "./assets/images/product/pair-trainers 1.png",
    name: "Name Of Product",
    description: "Name Of Producttt",
    price: "$19.5",
    rating: 4,
    reviewCount: "15",
    stock: "100",
  },
  {
    id: 2,
    image: "./assets/images/product/pair-trainers 2.png",
    name: "Name Of Product",
    description: "Name Of Producttt",
    price: "$19.5",
    rating: 4,
    reviewCount: "15",
    stock: "100",
  },
  {
    id: 3,
    image: "./assets/images/product/pair-trainers 3.png",
    name: "Name Of Product",
    description: "Name Of Producttt",
    price: "$19.5",
    rating: 4,
    reviewCount: "15",
    stock: "100",
  },
  {
    id: 4,
    image: "./assets/images/product/pair-trainers 4.png",
    name: "Name Of Product",
    description: "Name Of Producttt",
    price: "$19.5",
    rating: 4,
    reviewCount: "15",
    stock: "100",
  },
  {
    id: 5,
    image: "./assets/images/product/pair-trainers 5.png",
    name: "Name Of Product",
    description: "Name Of Producttt",
    price: "$19.5",
    rating: 4,
    reviewCount: "15",
    stock: "100",
  },
  {
    id: 6,
    image: "./assets/images/product/pair-trainers 6.png",
    name: "Name Of Product",
    description: "Name Of Producttt",
    price: "$19.5",
    rating: 4,
    reviewCount: "15",
    stock: "100",
  },
  {
    id: 7,
    image: "./assets/images/product/pair-trainers 7.png",
    name: "Name Of Product",
    description: "Name Of Producttt",
    price: "$19.5",
    rating: 4,
    reviewCount: "15",
    stock: "100",
  },
  {
    id: 8,
    image: "./assets/images/product/pair-trainers 8.png",
    name: "Name Of Product",
    description: "Name Of Producttt",
    price: "$19.5",
    rating: 4,
    reviewCount: "15",
    stock: "100",
  },
  {
    id: 9,
    image: "./assets/images/product/pair-trainers 1.png",
    name: "Name Of Product",
    description: "Name Of Producttt",
    price: "$19.5",
    rating: 4,
    reviewCount: "15",
    stock: "100",
  },
  {
    id: 10,
    name: "Name Of Product",
    image: "./assets/images/product/pair-trainers 2.png",
    description: "Name Of Producttt",
    price: "$19.5",
    rating: 4,
    reviewCount: "15",
    stock: "100",
  },
];

export default function Productv2Card() {
  const [currentPage, setCurrentPage] = useState(1);
  const productsPerPage = 8;
  const totalProducts = Math.min(80, products.length);

  const indexOfLastProduct = currentPage * productsPerPage;
  const indexOfFirstProduct = indexOfLastProduct - productsPerPage;
  const currentProducts = products.slice(
    indexOfFirstProduct,
    indexOfLastProduct
  );

  const handlePageChange = (pageNumber) => {
    setCurrentPage(pageNumber);
  };

  return (
    <>
      {currentProducts.map((product) => (
        <div className="flex mb-10" key={product.id}>
          <div className="w-1/4 p-0">
            <img
              src={product.image}
              alt="Product"
              className="rounded-lg w-full"
            />
          </div>
          <div className="w-1/3 pl-10">
            <h4 className="text-xl font-bold text-orange-500">SHOES</h4>
            <h2 className="text-2xl font-bold text-orange-600 underline">
              {product.name}
            </h2>
            <p className="text-gray-600 my-4">{product.description}</p>
            <ul className="list-disc list-inside mt-2 text-gray-700">
              <li>Available in colors</li>
              <li>Size ranges from 36 to 45</li>
            </ul>
          </div>
          <div className="w-px bg-gray-300 mx-6"></div>
          <div className="w-1/3 flex flex-col justify-between items-center">
            <div className="text-center">
              <div className="text-2xl font-bold text-black-800">
                {product.price}
              </div>
              <div className="flex items-center justify-center text-orange-500 mt-2">
                <Rating value={product.rating} readonly />
                <span className="text-gray-600 ml-2">
                  ({product.reviewCount})
                </span>
              </div>
              <div className="text-gray-600 mt-2">
                <span className="font-bold">Stock:</span> {product.stock}{" "}
                Available
              </div>
            </div>
            <div className="flex text-center mt-4 space-x-2 my-2 w-fit">
              <button className="bg-orange-500 hover:bg-orange-600 text-white font-bold rounded w-[200px] h-10">
                ADD TO CART
              </button>
              <div className="flex justify-center space-x-2 w-full">
                {/* <Button color="black">
                  <FontAwesomeIcon icon={faHeart} color="orange" />
                </Button> */}
                <button className="bg-orange-500 hover:bg-orange-600 text-white font-bold rounded w-10 h-10">
                  <FontAwesomeIcon icon={faHeart} color="black" />
                </button>
                {/* <Button color="black">
                  
                </Button> */}
                <button className="bg-orange-500 hover:bg-orange-600 text-white font-bold rounded w-10 h-10">
                  <FontAwesomeIcon icon={faShareNodes} color="black" />
                </button>
              </div>
            </div>
          </div>
        </div>
      ))}
      <Pagination
        total={totalProducts}
        current={currentPage}
        onChange={handlePageChange}
      />
    </>
  );
}
