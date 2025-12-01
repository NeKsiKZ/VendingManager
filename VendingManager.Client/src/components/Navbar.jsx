import React from 'react';
import { Search } from "lucide-react";

const Navbar = ({ searchTerm, setSearchTerm, statusFilter, setStatusFilter }) => {
  return (
    <div className="bg-white dark:bg-neutral-950/80 backdrop-blur-md border-b border-gray-200 dark:border-neutral-900 sticky top-0 z-20 transition-colors duration-300">
      <div className="max-w-7xl mx-auto px-4 py-6 sm:px-6 lg:px-8">
        <div className="flex flex-col xl:flex-row xl:items-center gap-6 justify-between">
          {/* Tytuł i Opis */}
          <div className="text-center xl:text-left min-w-[250px]">
            <h1 className="text-3xl md:text-4xl font-extrabold tracking-tight text-transparent bg-clip-text bg-gradient-to-r from-blue-600 to-indigo-600 pb-1">
              VendingManager
            </h1>
            <p className="text-sm text-gray-500 dark:text-gray-400 mt-1">
              Znajdź najbliższy automat i kup napój.
            </p>
          </div>

          {/* Pasek Wyszukiwania i Filtry */}
          <div className="flex flex-col md:flex-row gap-3 w-full xl:max-w-4xl">
            {/* Search Bar */}
            <div className="relative w-full md:flex-1 shadow-sm shadow-gray-200/50 dark:shadow-black/40 rounded-xl">
              <Search
                className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400"
                size={20}
              />
              <input
                type="text"
                className="w-full pl-11 pr-4 py-3 rounded-xl bg-gray-50 dark:bg-neutral-900 border border-gray-100 dark:border-neutral-800 focus:border-blue-500 dark:focus:border-blue-500 focus:ring-4 focus:ring-blue-500/10 outline-none transition-all placeholder-gray-400 dark:text-white text-base"
                placeholder="Szukaj automatu..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
              />
            </div>

            {/* Filtry */}
            <div className="grid grid-cols-3 gap-2 w-full md:w-auto md:flex md:justify-start shrink-0">
              {[
                { value: "All", label: "Wszystkie", icon: null },
                {
                  value: "Online",
                  label: "Online",
                  icon: <div className="w-2 h-2 rounded-full bg-green-500 animate-pulse mr-2" />,
                },
                {
                  value: "Offline",
                  label: "Offline",
                  icon: <div className="w-2 h-2 rounded-full bg-red-500 mr-2" />,
                },
              ].map((status) => (
                <button
                  key={status.value}
                  onClick={() => setStatusFilter(status.value)}
                  className={`
                    flex items-center justify-center px-2 sm:px-4 py-3 rounded-xl font-medium text-xs sm:text-sm transition-all duration-300 border whitespace-nowrap w-full md:w-auto
                    ${
                      statusFilter === status.value
                        ? "bg-neutral-900 dark:bg-white text-white dark:text-neutral-900 border-transparent shadow-md scale-[1.02]"
                        : "bg-gray-50 dark:bg-neutral-900 text-gray-600 dark:text-gray-400 border-gray-100 dark:border-neutral-800 hover:bg-gray-100 dark:hover:bg-neutral-800"
                    }
                  `}
                >
                  {status.icon}
                  {status.label}
                </button>
              ))}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Navbar;