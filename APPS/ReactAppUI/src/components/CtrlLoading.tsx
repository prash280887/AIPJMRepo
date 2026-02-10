export interface LoadingCardProps {
    isLoading: boolean;
}

const CtrlLoadingCard: React.FC<LoadingCardProps & { spinMessage?: string }> = ({ isLoading, spinMessage}) => {
    return (
        <>
            {isLoading && (
                 <div className="overlay">
                        <div className="spinner-container">
                            <div className="spinner"></div>
                            <p>{spinMessage}</p>
                        </div>
                        </div>

            )}
        </>
    );
};

export default CtrlLoadingCard;
