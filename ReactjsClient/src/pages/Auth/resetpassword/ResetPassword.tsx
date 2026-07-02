import { type ReactElement, useState } from "react";
import { isNullOrWhiteSpace } from "../../../utils/helperFunctions";
import { Link, useSearchParams } from "react-router-dom";
import { ResetPasswordHandler } from "../../../services/AuthService";

export default function ResetPassword(): ReactElement {
    type formState = {
        password: string,
        confirmPassword: string
    }
    const [loading, setLoading] = useState<boolean>();
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState<boolean>();
    const [form, setForm] = useState<formState>({
        password: "",
        confirmPassword: ""
    });


    function handleChange(e: React.ChangeEvent<HTMLInputElement>): void {
        const { name, value } = e.target;

        setForm(prev => ({
            ...prev,
            [name]: value
        }));
    }

    const [searchParam] = useSearchParams();
    const userId: string | null = searchParam.get("userId");
    const token: string | null = searchParam.get("token");

    async function SubmitHandler() {
        setLoading(true);
        if (isNullOrWhiteSpace(userId) || isNullOrWhiteSpace(token) || isNullOrWhiteSpace(form.password) || isNullOrWhiteSpace(form.confirmPassword) || form.confirmPassword != form.confirmPassword) {
            setLoading(false);
            setError("Unable to reset password. Check the passwords match and met all the constraints. If not try sending a new reset password to your email.");
            return;
        }

        var response = await ResetPasswordHandler(userId!, token!, form.password);
        if (response) {
            setSuccess(true);
        } else {
            setSuccess(false);
            setError("Unable to reset password. Check the passwords match and met all the constraints. If not try sending a new reset password to your email.");
        }

        setLoading(false);
    }
    return (
        <div className="reset-password-container">

            <div className="reset-password-wrapper">
                <div className="logo">
                    <h1>ReviewAnything</h1>
                </div>

                {success
                    ?
                    <div className="success-message">
                        Your password has been reset successfully. You can now sign in with your new password.
                    </div>
                    :
                    <>
                        <h2 className="subtitle">Reset Your Password</h2>

                        <div className="password-requirements">
                            <h4>Password Requirements:</h4>
                            <ul>
                                <li>At least 8 characters long</li>
                                <li>Contains uppercase and lowercase letters</li>
                                <li>Contains at least one number</li>
                                <li>Contains at least one special character</li>
                            </ul>
                        </div>

                        <form onSubmit={SubmitHandler}>

                            <div className="form-group">
                                <label htmlFor="newPassword">New Password</label>
                                <div className="input-wrapper">
                                    <input type="password"
                                        placeholder="Password" className="form-input" id="newPassword" name="Password" onChange={handleChange} />
                                </div>
                            </div>

                            <div className="form-group">
                                <label htmlFor="confirmPassword">Confirm Password</label>
                                <div className="input-wrapper">
                                    <input type="password"
                                        placeholder="Confirm Password" className="form-input" id="confirmPassword" name="ConfirmPassword" onChange={handleChange} />
                                </div>
                            </div>

                            <button type="submit" className="btn-primary" disabled={loading}>
                                {loading ?
                                    <div className="spinner-container">
                                        <span className="spinner"></span>
                                        <span>Loading...</span>
                                    </div>
                                    :
                                    <span>Reset Password</span>
                                }
                            </button>
                            {!isNullOrWhiteSpace(error) &&
                                <div className="error-message">{error}</div>
                            }
                        </form>
                    </>
                }

                <div className="back-link">
                    <Link to="/login">← Back to Login</Link>
                </div>
            </div>
        </div>
    )
}