import { useEffect, useState } from "react";

type SearchProps<T> = {
    collection: string;
    onResults: (results: T[]) => void; 
};

function Search<T>({collection, onResults}: SearchProps<T>) {
    const [query, setQuery] = useState<string>("");
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (query.trim().length < 2) {
            onResults([]);
            return;
        }

        setTimeout(async () => {
            setLoading(true);
            try {
                const response: Response = await fetch(
                    `${process.env.REACT_APP_API_BASE_URL}/api/${collection}/search?query=${encodeURIComponent(query)}`, {
                        headers: {
                            Authorization: `Bearer ${localStorage.getItem("token")}`
                        }
                });
                const data: T[] = await response.json();

                if (!response.ok) throw new Error(`${response.status}`);

                onResults(data);

            } catch (error: any) {
                setError(error.toString());
                console.log(error);
            } finally {
                setLoading(false);
            }
        }, 800);

    }, [query, collection, onResults]);

    if (error) return <p>{`Error during search ${error}`}</p>

    return (
        <div className="inline-search">
            <form id="search-form" className="search-form"> 
                <input 
                    className="input"
                    type="search" 
                    id="query"
                    name="q" 
                    placeholder={`Search ${collection}...`}
                    onChange={(e) => setQuery(e.target.value)}
                />
            </form>
            {loading && <p className="muted">Searching...</p>}
        </div>
    )
}

export default Search;