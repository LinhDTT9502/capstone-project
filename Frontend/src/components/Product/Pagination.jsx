import PropTypes from "prop-types";

function Pagination({ total, current, onChange }) {
  const totalPages = Math.ceil(total / 10); 
  const getPageNumbers = () => {
    const pageNumbers = [];
    if (totalPages <= 5) {
      for (let i = 1; i <= totalPages; i++) {
        pageNumbers.push(i);
      }
    } else {
      if (current <= 3) {
        pageNumbers.push(1, 2, 3, 4, "...", totalPages);
      } else if (current > totalPages - 3) {
        pageNumbers.push(
          1,
          "...",
          totalPages - 3,
          totalPages - 2,
          totalPages - 1,
          totalPages
        );
      } else {
        pageNumbers.push(
          1,
          "...",
          current - 1,
          current,
          current + 1,
          "...",
          totalPages
        );
      }
    }
    return pageNumbers;
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
            onClick={() => onChange(current - 1)}
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
            {typeof page === "number" ? (
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
            ) : (
              <span className="px-3 py-2 leading-tight text-gray-500 bg-white border border-gray-300">
                {page}
              </span>
            )}
          </li>
        ))}
        <li>
          <button
            onClick={() => onChange(current + 1)}
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

Pagination.propTypes = {
  total: PropTypes.number.isRequired,
  current: PropTypes.number.isRequired,
  onChange: PropTypes.func.isRequired,
};

export default Pagination;
