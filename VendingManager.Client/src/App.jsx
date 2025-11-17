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
      if (!response.ok) {
        throw new Error('Błąd sieci lub zły klucz API');
      }
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

  if (loading) return <h1>Ładowanie danych z .NET...</h1>;
  if (error) return <h1 style={{color: 'red'}}>Błąd: {error}</h1>;

  return (
    <div style={{ padding: '20px', fontFamily: 'Arial' }}>
      <h1>VendingManager - React Frontend</h1>
      <h2>Lista Automatów (Pobrana z API)</h2>

      <div style={{ display: 'flex', gap: '20px', flexWrap: 'wrap' }}>
        {machines.map(machine => (
          <div key={machine.id} style={{
            border: '1px solid #ccc',
            borderRadius: '8px',
            padding: '20px',
            width: '250px',
            boxShadow: '0 4px 8px rgba(0,0,0,0.1)'
          }}>
            <h3>{machine.name}</h3>
            <p>{machine.location}</p>
            <p>
              Status: 
              <span style={{ 
                fontWeight: 'bold', 
                color: machine.status === 'Online' ? 'green' : 'red',
                marginLeft: '5px'
              }}>
                {machine.status}
              </span>
            </p>
          </div>
        ))}
      </div>
    </div>
  )
}

export default App