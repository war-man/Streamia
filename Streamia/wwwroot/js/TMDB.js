class TMDB {
    constructor(apiKey) {
        this.key = apiKey;
        this.baseUrl = 'https://api.themoviedb.org/3/';
    }

    search(keyword, type, callback) {
        fetch(`${this.baseUrl}search/${type}${this.apiKeyQuery()}${this.searchQuery(keyword)}`)
            .then(res => res.json())
            .then(data => callback(data));
    }

    find(path, append, callback) {
        fetch(`${this.baseUrl}${path}${this.apiKeyQuery()}${append ? this.appendQuery(append) : ''}`)
            .then(res => res.json())
            .then(data => callback(data));
    }

    generatePosterUrl(posterPath) {
        return `https://image.tmdb.org/t/p/w600_and_h900_bestv2${posterPath}`;
    }

    apiKeyQuery() {
        return `?api_key=${this.key}`;
    }

    searchQuery(keyword) {
        return `&query=${keyword}`;
    }

    appendQuery(append) {
        return `&append_to_response=${append}`;
    }
}