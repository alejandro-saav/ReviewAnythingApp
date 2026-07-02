import { type ReactElement, useState } from "react";
import { isNullOrWhiteSpace } from "../../../utils/helperFunctions";
import styles from './SignUp.module.css';
import type { RegisterRequest } from "../../../types/AuthTypes";
import { useForm, type SubmitHandler } from "react-hook-form";
import { RegisterUserHandler } from "../../../services/AuthService";
import { useNavigate } from "react-router-dom";

export default function SignUp(): ReactElement {
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    const [imageErrorMessage, setImageErrorMessage] = useState<string | null>(null);
    const [imageNameSelected, setImageNameSelected] = useState<string | null>(null);
    const [imagePreviewUrl, setImagePreviewUrl] = useState<string | undefined>(undefined);

    const navigate = useNavigate();

    const { register, handleSubmit, formState: { errors }, } = useForm<RegisterRequest>();

    const onSubmit: SubmitHandler<RegisterRequest> = async (data) => {
        setIsLoading(true);
        const successResponse = await RegisterUserHandler(data);

        if (successResponse) {
            navigate("/email-confirmation-required");
            return;
        } else {
            setErrorMessage("Something went wrong, please try again.");
        }
        setIsLoading(false);
    }

    function HandleRemovePhoto() {
        setImageNameSelected(null);
        setImagePreviewUrl("");
    }

    function HandleImageSelected(e: React.ChangeEvent<HTMLInputElement>) {
        setImageErrorMessage(null);
        setImageNameSelected(null);
        const maxFileSize: number = 1024 * 1024 * 2;

        const file: File | undefined = e.target.files?.[0];
        if (!file || file === undefined) {
            setImageErrorMessage("No file found.");
        };
        if (file!.size > maxFileSize) {
            setImageErrorMessage("File is too big, max size is 2MB");
            return;
        }

        if (file!.type.substring(0, 5) != "image") {
            setImageErrorMessage("File type is not supported. Only images are supported.");
            return;
        }

        const reader = new FileReader();
        reader.onloadend = () => {
            const imageName = file!.name;
            const imageExtension = imageName.substring(imageName.lastIndexOf("."));
            const imageNameTruncated = imageName.length <= 15 ? imageName : `${imageName.substring(0, 10)}...${imageExtension}`;
            setImageNameSelected(imageNameTruncated);
            setImagePreviewUrl(reader.result as string);
        }
        reader.readAsDataURL(file!);
    }
    return (
        <div className={styles.signupContainer}>
            <div className={styles.signupWrapper}>
                <div className={styles.signupPanel}>
                    <h2 className={styles.panelTitle}>Sign up</h2>
                    <form onSubmit={handleSubmit(onSubmit)}>
                        <div className={styles.formContainer}>
                            <div className={styles.requiredFieldsContainer}>
                                <div className={styles.formGroup}>
                                    <label htmlFor="username">Username:<span className={styles.asterisk}> *</span></label>
                                    <input {...register("userName", { required: { value: true, message: "Username is required" }, maxLength: { value: 20, message: "Max 20 characters." }, minLength: { value: 5, message: "Min 5 characters." } })} type="text" placeholder="Username"
                                        className={styles.formInput} id="username" name="UserName" />
                                    {errors.userName && <div className={styles.validationError}>{errors.userName.message}</div>}
                                </div>

                                <div className={styles.formGroup}>
                                    <label htmlFor="firstname">Firstname:<span className={styles.asterisk}> *</span></label>
                                    <input {...register("firstName", { required: { value: true, message: "Firstname is required" }, maxLength: { value: 30, message: "Max 30 characters" }, minLength: { value: 3, message: "Min 3 characters." } })} type="text"
                                        placeholder="Firstname" className={styles.formInput} id="firstname" name="FirstName" />
                                    {errors.firstName && <div className={styles.validationError}>{errors.firstName.message}</div>}
                                </div>

                                <div className={styles.formGroup}>
                                    <label htmlFor="email">Email:<span className={styles.asterisk}> *</span></label>
                                    <input {...register("email", { required: { value: true, message: "Email is required" } })} type="email"
                                        placeholder="Email Address" className={styles.formInput} id="email" name="Email" />
                                    {errors.email && <div className={styles.validationError}>{errors.email.message}</div>}
                                </div>

                                <div className={styles.formGroup}>
                                    <label htmlFor="password">Password:<span className={styles.asterisk}> *</span></label>
                                    <input {...register("password", { required: { value: true, message: "Password is required" }, minLength: { value: 8, message: "Min 8 characters" }, maxLength: { value: 50, message: "Max 50 characters" }, pattern: { value: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).+$/, message: "Password must contain at least one upper and lower case letter, one digit and one special character" } })} type="password"
                                        placeholder="Password" className={styles.formInput} id="password" name="Password" />
                                    {errors.password && <div className={styles.validationError}>{errors.password.message}</div>}
                                </div>
                            </div>
                            <div className={styles.optionalFieldsContainer}>
                                <div className={styles.formGroup}>
                                    <label htmlFor="lastname">Lastname:</label>
                                    <input {...register("lastName", { maxLength: { value: 30, message: "Max 30 characters" } })} type="text" placeholder="LastName"
                                        className={styles.formInput} id="lastname" name="LastName" />
                                    {errors.lastName && <div className={styles.validationError}>{errors.lastName.message}</div>}
                                </div>

                                <div className={styles.formGroup}>
                                    <label htmlFor="phone">Phone Number:</label>
                                    <input {...register("phone")} type="text"
                                        placeholder="Phone number" className={styles.formInput} id="phone" name="Phone" />
                                </div>

                                <div className={styles.uploadContainer}>
                                    <label htmlFor="profile-upload" className={styles.profileUpload}>
                                        Profile Picture: <span className={styles.imageName}>{imageNameSelected}</span>
                                    </label>
                                    {isNullOrWhiteSpace(imagePreviewUrl) ?
                                        <label className={styles.svgContainer} htmlFor="profile-upload">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24"
                                                fill="none">
                                                <circle cx="12" cy="12" r="11" stroke="#FE5D26" strokeWidth="2" fill="none" />
                                                <path
                                                    d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z"
                                                    fill="#FE5D26" />
                                            </svg>
                                            <span className={styles.plusSign}>+</span>
                                        </label>
                                        :
                                        <label htmlFor="profile-upload">
                                            <div className={styles.imagePreview} id="imagePreview">
                                                <img id="previewImg" src={imagePreviewUrl} alt="Profile Preview" />
                                                <button type="button" className={styles.removeBtn} onClick={HandleRemovePhoto}>×</button>
                                            </div>
                                        </label>
                                    }
                                    <input {...register("profileImage")} id="profile-upload" accept="image/*" onChange={HandleImageSelected}
                                        style={{ display: "none" }} name="ProfileImage" type="file" />
                                </div>

                                {!isNullOrWhiteSpace(imageErrorMessage) &&
                                    <div className={`${styles.errorMessage} ${styles.imgErrorMessage}`} style={{ marginBottom: "10px" }}>{imageErrorMessage}
                                    </div>
                                }
                                <div className={styles.formGroup}>
                                    <label htmlFor="bio">Biography:</label>
                                    <textarea {...register("bio", { maxLength: { value: 500, message: "Max 500 characters" } })}
                                        placeholder="Bio max length(500)" className={styles.formInput} id="bio" maxLength={500}
                                        style={{ resize: "none" }} name="Bio"></textarea>
                                    {errors.bio && <div className={styles.validationError}>{errors.bio.message}</div>}
                                </div>
                            </div>
                        </div>
                        <button type="submit" className={styles.btnPrimary} disabled={isLoading}>
                            {isLoading ?
                                <div className={styles.spinnerContainer}>
                                    <span className={styles.spinner}></span>
                                    <span>Loading...</span>
                                </div>
                                :
                                <span>Signup</span>
                            }
                        </button>

                        {!isNullOrWhiteSpace(errorMessage) &&
                            <div className={styles.errorMessage}>{errorMessage}</div>
                        }
                    </form>
                </div>
            </div>
        </div>
    )
}