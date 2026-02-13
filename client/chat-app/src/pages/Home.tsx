import { ReactElement } from 'react';
import { useNavigate } from 'react-router-dom';

function Home(): ReactElement {
    const navigate = useNavigate();

    const handleClick = (path: string) => {
        navigate(path);
    };

    return (
        <section className="home-page">
            <div className="card home-card">
                <h2>Real-time Chat Application</h2>
                <p className="home-subtitle">Simple, fast messaging between you and your contacts.</p>
                <div className="home-actions">
                    <button className='btn btn-primary' onClick={() => handleClick('/login')}>Login</button>
                    <button className='btn btn-secondary' onClick={() => handleClick('/register')}>Register</button>
                </div>
            </div>
        </section>
    );
};

export default Home;