import PropTypes from "prop-types";

function ProductCard({ product }) {
  return (
    <div className="relative group border border-gray-200 rounded-lg overflow-hidden">
      <img
        src={product.image}
        alt={product.name}
        className="w-full h-100 object-cover"
      />
      <div className="p-4">
        {/* <h3 className="text-sm text-orange-500">{product.category}</h3> */}
        <h2 className="mt-1 text-lg font-semibold text-gray-900">
          {product.name}
        </h2>
        <p className="mt-1 text-sm text-gray-600">${product.price}</p>
      </div>
      <div className="absolute mt-[14rem] inset-0 flex items-center justify-center bg-black bg-opacity-0 opacity-0 group-hover:opacity-100 transition-opacity">
        <button className="w-full px-8 py-4 bg-[#3E3E3EE5] text-black font-semibold ">
          ADD TO CART
        </button>
      </div>
      <div className="absolute top-4 right-4 flex flex-col space-y-2 opacity-0 group-hover:opacity-100 transition-opacity">
        <button className="p-2 bg-white rounded-full shadow">
          <svg
            className="w-4 h-4"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              d="M5 13l4 4L19 7"
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth="2"
            ></path>
          </svg>
        </button>
        <button className="p-2 bg-white rounded-full shadow">
          <svg
            className="w-4 h-4"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              d="M20 12H4"
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth="2"
            ></path>
          </svg>
        </button>
        <button className="p-2 bg-white rounded-full shadow">
          <svg
            className="w-4 h-4"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              d="M19 21l-7-5-7 5V5a2 2 0 012-2h10a2 2 0 012 2z"
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth="2"
            ></path>
          </svg>
        </button>
      </div>
    </div>
  );
}

ProductCard.propTypes = {
  product: PropTypes.shape({
    category: PropTypes.string.isRequired,
    name: PropTypes.string.isRequired,
    price: PropTypes.number.isRequired,
    image: PropTypes.string.isRequired,
  }).isRequired,
};

export default ProductCard;
