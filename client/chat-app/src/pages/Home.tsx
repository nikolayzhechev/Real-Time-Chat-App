import { ReactElement } from 'react';
import { useNavigate } from 'react-router-dom';

function Home(): ReactElement {
    const navigate = useNavigate();

    const handleClick = (path: string) => {
        navigate(path);
    };

    return (
        <div>
            <h2>Real-time Chat Application</h2>
            <button className='login_btn' onClick={() => handleClick('/login')}>Login</button>
            <button className='register_btn' onClick={() => handleClick('/register')}>Register</button>
        </div>
    )
};

export default Home;