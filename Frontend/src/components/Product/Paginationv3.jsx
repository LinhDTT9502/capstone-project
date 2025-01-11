const Paginationv3 = ({ currentPage, totalPages, onPageChange }) => {
    const getPageNumbers = () => {
      const pageNumbers = [];
      const maxVisiblePages = 5;
  
      if (totalPages <= maxVisiblePages) {
        for (let i = 1; i <= totalPages; i++) {
          pageNumbers.push(i);
        }
      } else {
        if (currentPage <= 3) {
          for (let i = 1; i <= 4; i++) {
            pageNumbers.push(i);
          }
          pageNumbers.push('...');
          pageNumbers.push(totalPages);
        } else if (currentPage >= totalPages - 2) {
          pageNumbers.push(1);
          pageNumbers.push('...');
          for (let i = totalPages - 3; i <= totalPages; i++) {
            pageNumbers.push(i);
          }
        } else {
          pageNumbers.push(1);
          pageNumbers.push('...');
          for (let i = currentPage - 1; i <= currentPage + 1; i++) {
            pageNumbers.push(i);
          }
          pageNumbers.push('...');
          pageNumbers.push(totalPages);
        }
      }
  
      return pageNumbers;
    };
  
    return (
      <nav className="flex items-center justify-end space-x-2 py-4">
        <div
          className={`w-8 h-8 flex items-center justify-center rounded-full ${
            currentPage === 1
              ? 'bg-gray-100 text-gray-400 cursor-not-allowed'
              : 'bg-white text-blue-600 hover:bg-blue-100 cursor-pointer'
          }`}
          onClick={() => currentPage !== 1 && onPageChange(currentPage - 1)}
        >
          &#8249;
        </div>
        {getPageNumbers().map((pageNumber, index) => (
          <div
            key={index}
            className={`w-8 h-8 flex items-center justify-center rounded-full ${
              pageNumber === currentPage
                ? 'bg-blue-600 text-white'
                : pageNumber === '...'
                ? 'cursor-default'
                : 'bg-white text-blue-600 hover:bg-blue-100 cursor-pointer'
            }`}
            onClick={() => typeof pageNumber === 'number' && onPageChange(pageNumber)}
          >
            {pageNumber}
          </div>
        ))}
        <div
          className={`w-8 h-8 flex items-center justify-center rounded-full ${
            currentPage === totalPages
              ? 'bg-gray-100 text-gray-400 cursor-not-allowed'
              : 'bg-white text-blue-600 hover:bg-blue-100 cursor-pointer'
          }`}
          onClick={() => currentPage !== totalPages && onPageChange(currentPage + 1)}
        >
          &#8250;
        </div>
      </nav>
    );
  };
  
  export default Paginationv3;
  
  