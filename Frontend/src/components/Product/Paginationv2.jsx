import PropTypes from "prop-types";

function Paginationv2({ current, onChange }) {
  const totalPages = 8;

  const getPageNumbers = () => {
    const pages = [];
    for (let i = 1; i <= totalPages; i++) {
      pages.push(i);
    }
    return pages;
  };

  const pageNumbers = getPageNumbers();

  return (
    <nav className="flex justify-center mt-8">
      <ul className="inline-flex items-center -space-x-px">
        <li>
          <button
            onClick={() => onChange(1)}
            disabled={current === 1}
            className={`px-3 py-2 leading-tight ${
              current === 1
                ? "text-gray-300"
                : "text-gray-500 hover:text-gray-700"
            } bg-white border border-gray-300 rounded-l-lg`}
          >
            «
          </button>
        </li>
        <li>
          <button
            onClick={() => onChange(Math.max(1, current - 1))}
            disabled={current === 1}
            className={`px-3 py-2 leading-tight ${
              current === 1
                ? "text-gray-300"
                : "text-gray-500 hover:text-gray-700"
            } bg-white border border-gray-300`}
          >
            ‹
          </button>
        </li>
        {pageNumbers.map((page, index) => (
          <li key={index}>
            <button
              onClick={() => onChange(page)}
              className={`px-3 py-2 leading-tight ${
                page === current
                  ? "text-white bg-gray-800"
                  : "text-gray-500 hover:text-gray-700"
              } bg-white border border-gray-300`}
            >
              {page}
            </button>
          </li>
        ))}
        <li>
          <button
            onClick={() => onChange(Math.min(totalPages, current + 1))}
            disabled={current === totalPages}
            className={`px-3 py-2 leading-tight ${
              current === totalPages
                ? "text-gray-300"
                : "text-gray-500 hover:text-gray-700"
            } bg-white border border-gray-300`}
          >
            ›
          </button>
        </li>
        <li>
          <button
            onClick={() => onChange(totalPages)}
            disabled={current === totalPages}
            className={`px-3 py-2 leading-tight ${
              current === totalPages
                ? "text-gray-300"
                : "text-gray-500 hover:text-gray-700"
            } bg-white border border-gray-300 rounded-r-lg`}
          >
            »
          </button>
        </li>
      </ul>
    </nav>
  );
}

Paginationv2.propTypes = {
  total: PropTypes.number.isRequired,
  current: PropTypes.number.isRequired,
  onChange: PropTypes.func.isRequired,
};

export default Paginationv2;
