import { FormEvent, ReactElement, useState } from "react";
import { useUser } from "../contexts/userContext";

function Profile():ReactElement {
    const { authData } = useUser();
    const [passwordToggle, setPasswordToggle] = useState(false);
    const [emailToggle, setEmailToggle] = useState(false);
    const [password, setPassword] = useState('');
    const [repeatPassword, setRepeatPassword] = useState('');
    const [error, setError] = useState('');

    const handleSubmitPasswordChange = async (event: FormEvent<HTMLFormElement>): Promise<void> => {
        event.preventDefault();
        if (password != repeatPassword) {
            setError("Passwords don't match.");
        } else {
            // API request
    
            setPasswordToggle(prev => !prev);
            // clear inputs
            // clear error
        }
    };

    return (
        <section className="profile-page">
            <div className="card profile-card">
                <h2>User Profile</h2>
                <div className="profile-info">
                    <div className="profile-row">
                        <span className="profile-label">Username</span>
                        <span className="profile-value">{ authData?.username }</span>
                    </div>
                    <div className="profile-row">
                        <span className="profile-label">Email</span>
                        <span className="profile-value">{ authData?.email }</span>
                    </div>
                </div>

                <h3 className="profile-actions-title">Profile Actions</h3>
                <div>
                    {!passwordToggle ? (
                        <button
                            className="btn btn-secondary"
                            onClick={() => setPasswordToggle(prev => !prev)}
                        >
                            Change Password
                        </button>
                    ) : (
                        <form className="form" onSubmit={handleSubmitPasswordChange}>
                            <label className="form-label">New Password</label>
                            <input
                                className="input"
                                required
                                type="password"
                                name="newPassword"
                                placeholder="Enter new password"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                            />
                            <label className="form-label">Repeat Password</label>
                            <input
                                className="input"
                                required
                                type="password"
                                name="repeatNewPassword"
                                placeholder="Repeat password"
                                value={repeatPassword}
                                onChange={(e) => setRepeatPassword(e.target.value)}
                            />
                            {error && <p className="form-error">{error}</p>}
                            <button className="btn btn-primary" type="submit" name="submit">
                                Submit
                            </button>
                        </form>
                    )}
                </div>
            </div>
        </section>
    );
}

export default Profile;