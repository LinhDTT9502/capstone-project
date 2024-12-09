import React from "react";
import PolicyList from "./PolicyList";

const PolicyLayout = ({ children, title }) => {
  return (
    <div className="bg-gray-100 min-h-screen px-4 sm:px-6 lg:px-8 py-8 sm:py-12">
      <div className="max-w-7xl mx-auto">
        <div className="flex flex-col md:flex-row gap-8">
          <PolicyList />
          <div className="flex-1 bg-white rounded-lg shadow-md p-6 sm:p-8">
            <h1 className="text-3xl sm:text-4xl font-bold mb-6 text-gray-800 border-b pb-2">{title}</h1>
            {children}
          </div>
        </div>
      </div>
    </div>
  );
};

export default PolicyLayout;
