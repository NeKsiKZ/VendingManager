import { useState, useEffect } from 'react'

function App() {
  const [machines, setMachines] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const API_URL = import.meta.env.VITE_API_URL;
  const API_KEY = import.meta.env.VITE_API_KEY;

  useEffect(() => {
    fetch(API_URL, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'X-API-KEY': API_KEY
      }
    })
    .then(response => {
      if (!response.ok) throw new Error('Błąd sieci lub zły klucz API');
      return response.json();
    })
    .then(data => {
      setMachines(data);
      setLoading(false);
    })
    .catch(err => {
      setError(err.message);
      setLoading(false);
    });
  }, []);

  if (loading) return (
    <div className="d-flex justify-content-center align-items-center vh-100">
      <div className="spinner-border text-primary" role="status">
        <span className="visually-hidden">Loading...</span>
      </div>
    </div>
  );

  if (error) return (
    <div className="container mt-5">
      <div className="alert alert-danger" role="alert">
        Błąd: {error}
      </div>
    </div>
  );

  return (
    <div className="container py-5">
      {/* Nagłówek */}
      <div className="text-center mb-5">
        <h1 className="display-4 fw-bold">VendingManager</h1>
        <p className="lead text-muted">Publiczny podgląd statusu automatów</p>
      </div>

      {/* Siatka Kart (Grid) */}
      <div className="row g-4">
        {machines.map(machine => (
          <div key={machine.id} className="col-md-6 col-lg-4">
            <div className="card h-100 shadow-sm border-0">
              
              <div className="card-body">
                <div className="d-flex justify-content-between align-items-start mb-2">
                  <h5 className="card-title fw-bold text-primary">{machine.name}</h5>
                  {/* Badge statusu */}
                  <span className={`badge ${machine.status === 'Online' ? 'bg-success' : 'bg-danger'}`}>
                    {machine.status}
                  </span>
                </div>
                
                <h6 className="card-subtitle mb-3 text-muted">
                  <i className="bi bi-geo-alt-fill me-1"></i>
                  {machine.location}
                </h6>

                <p className="card-text small text-muted">
                  Ostatni kontakt: <br/>
                  {new Date(machine.lastContact).toLocaleString()}
                </p>
              </div>

              <div className="card-footer bg-transparent border-top-0">
                <button className="btn btn-outline-primary w-100 btn-sm">
                  Zobacz asortyment
                </button>
              </div>

            </div>
          </div>
        ))}
      </div>
    </div>
  )
}

export default App