import React from 'react';

const MachineSkeleton = () => (
  <div className="bg-white dark:bg-neutral-900 rounded-2xl p-6 shadow-sm border border-gray-100 dark:border-neutral-800 flex flex-col h-full animate-pulse">
    <div className="flex justify-between items-start mb-4">
      <div className="w-12 h-12 bg-gray-200 dark:bg-neutral-800 rounded-xl"></div>
      <div className="w-16 h-6 bg-gray-200 dark:bg-neutral-800 rounded-full"></div>
    </div>
    <div className="h-6 bg-gray-200 dark:bg-neutral-800 rounded w-3/4 mb-2"></div>
    <div className="h-4 bg-gray-200 dark:bg-neutral-800 rounded w-1/2 mb-6"></div>
    <div className="mt-auto h-12 bg-gray-200 dark:bg-neutral-800 rounded-xl w-full"></div>
  </div>
);

export default MachineSkeleton;