(function () {
    'use strict';

    // Add to module
    angular
        .module('app.logbook')
        .factory('cacheService', cacheService);

    // Inject dependencies
    cacheService.$inject = ['$resource', 'CacheFactory'];

    // Define service
    function cacheService($resource, CacheFactory) {

        // Define attributes
        var resourceCache;

        // Define the service
        var service = {
            createResource: createResource,
            flushCache: flushCache,
            flushCacheEntry: flushCacheEntry,
            flushCacheMatches: flushCacheMatches
        };
        initialize();
        return service;

        // Initialize the service
        function initialize() {
            if (!CacheFactory.get('resourceCache')) {
                CacheFactory.createCache('resourceCache', {
                    deleteOnExpire: 'aggressive',
                    recycleFreq: 60000
                });
            }
            resourceCache = CacheFactory.get('resourceCache');
        }

        // Create a resource
        function createResource(url, paramDefaults, actions, options) {
            actions = angular.merge({},
                {
                    'add': { method: 'POST', headers: { 'Content-Type': 'application/json; charset=utf-8' } },
                    'delete': { method: 'DELETE' },
                    'get': { method: 'GET', cache: resourceCache },
                    'query': {
                        method: 'GET',
                        cache: resourceCache,
                        isArray: false,
                        transformResponse: function (data, headers) {
                            try {
                                var response = {};
                                response.data = angular.fromJson(data); //JSON.parse(data);
                                response.headers = headers();
                                return response;
                            } catch (e) {
                                return data;
                            }
                        }
                    },
                    'update': { method: 'PUT' }
                }, actions
            );
            return $resource(url, paramDefaults, actions, options);
        }

        // Flush the cache
        function flushCache() {
            if (angular.isDefined(resourceCache)) {
                resourceCache.removeAll();
                console.log('Flushed cache');
            }
        }

        // Flush cache entry
        function flushCacheEntry(key) {
            if (angular.isDefined(resourceCache)) {
                resourceCache.remove(key);
                console.log('Flushed cache entry: ' + key);
                console.log(resourceCache.keys());
            }
        }

        // Flush matching cache entries
        function flushCacheMatches(match) {
            if (angular.isDefined(resourceCache)) {
                var keys = resourceCache.keys();
                for (var i = 0; i < keys.length; i++) {
                    if (keys[i].indexOf(match) > -1) {
                        resourceCache.remove(keys[i]);
                        console.log('Flushed cache entry: ' + keys[i]);
                    }
                }
                console.log(resourceCache.keys());
            }
        }
    }
})();
