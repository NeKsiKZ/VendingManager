import React from 'react';
import { X, Loader2, Plus } from "lucide-react";

const ProductModal = ({ machine, products, loading, onClose, onAddToCart }) => {
  if (!machine) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
      <div
        className="absolute inset-0 bg-black/60 backdrop-blur-sm"
        onClick={onClose}
      ></div>

      <div className="relative bg-white dark:bg-neutral-900 rounded-3xl shadow-2xl w-full max-w-4xl max-h-[90vh] flex flex-col overflow-hidden animate-in fade-in zoom-in duration-300 border dark:border-neutral-800">
        <div className="p-6 border-b border-gray-100 dark:border-neutral-800 flex justify-between items-center bg-gray-50 dark:bg-neutral-900">
          <div>
            <h3 className="text-2xl font-bold text-gray-800 dark:text-white">
              {machine.name}
            </h3>
            <div className="flex items-center gap-2 mt-1">
              <span
                className={`w-2.5 h-2.5 rounded-full ${
                  machine.status === "Online" ? "bg-green-500" : "bg-red-500"
                }`}
              ></span>
              <span className="text-sm text-gray-500 dark:text-neutral-400">
                {machine.status} • {machine.location}
              </span>
            </div>
          </div>
          <button
            onClick={onClose}
            className="p-2 hover:bg-gray-200 dark:hover:bg-neutral-800 rounded-full transition-colors text-gray-500 dark:text-neutral-400"
          >
            <X size={24} />
          </button>
        </div>

        <div className="flex-1 overflow-y-auto p-6 bg-gray-50/30 dark:bg-neutral-950">
          {loading ? (
            <div className="flex flex-col items-center justify-center py-20 text-blue-600 dark:text-blue-400">
              <Loader2 className="animate-spin mb-4" size={40} />
              <p>Ładowanie produktów...</p>
            </div>
          ) : (
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
              {products.map((p) => (
                <div
                  key={p.productId}
                  className="bg-white dark:bg-neutral-900 rounded-xl p-4 shadow-sm border border-gray-100 dark:border-neutral-800 hover:shadow-md transition-shadow flex gap-4 items-center"
                >
                  <div className="w-20 h-20 bg-gray-100 dark:bg-neutral-800 rounded-lg flex items-center justify-center overflow-hidden flex-shrink-0 group">
                    {p.imageUrl ? (
                      <img
                        src={p.imageUrl}
                        alt={p.productName}
                        className="w-full h-full object-cover img-zoom"
                      />
                    ) : (
                      <span className="text-xs text-gray-400 text-center px-1">
                        Brak zdjęcia
                      </span>
                    )}
                  </div>
                  <div className="flex-1 min-w-0">
                    <h4 className="font-bold text-gray-800 dark:text-gray-100 truncate">
                      {p.productName}
                    </h4>
                    <div className="flex items-baseline gap-2 mb-2">
                      <span className="text-lg font-bold text-blue-600 dark:text-blue-400">
                        {p.price.toFixed(2)} zł
                      </span>
                      <span className="text-xs text-gray-400">/ szt.</span>
                    </div>

                    {p.quantity > 0 ? (
                      <button
                        onClick={() => onAddToCart(p)}
                        className="w-full bg-gray-900 dark:bg-blue-600 text-white text-sm py-2 rounded-lg hover:bg-blue-600 dark:hover:bg-blue-500 transition-colors flex items-center justify-center gap-2"
                      >
                        <Plus size={16} /> Dodaj
                      </button>
                    ) : (
                      <button
                        disabled
                        className="w-full bg-gray-100 dark:bg-neutral-800 text-gray-400 text-sm py-2 rounded-lg cursor-not-allowed"
                      >
                        Wyprzedane
                      </button>
                    )}
                    <div className="mt-1 text-right">
                      <span
                        className={`text-[10px] uppercase font-bold tracking-wider ${
                          p.quantity < 5
                            ? "text-red-500 dark:text-red-400"
                            : "text-green-500 dark:text-green-400"
                        }`}
                      >
                        {p.quantity} szt.
                      </span>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default ProductModal;