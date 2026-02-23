namespace SweetShop.Middleware;

/// <summary>
/// Injects hardened security headers into every HTTP response.
/// Register this BEFORE UseStaticFiles and UseRouting so that even
/// static assets and error pages carry the headers.
/// </summary>
public class SecurityHeadersMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // ── Run headers BEFORE the response body is written ───────────
        context.Response.OnStarting(() =>
        {
            var h = context.Response.Headers;

            // ── 1. Clickjacking Protection ─────────────────────────────
            // Prevents the page from being embedded in any <iframe> or <object>.
            h["X-Frame-Options"] = "DENY";

            // ── 2. MIME-Type Sniffing Protection ───────────────────────
            // Forces the browser to honour the declared Content-Type.
            // Blocks attacks where an image upload is misinterpreted as a script.
            h["X-Content-Type-Options"] = "nosniff";

            // ── 3. HTTP Strict Transport Security (HSTS) ───────────────
            // max-age=31536000  → enforce HTTPS for 1 year
            // includeSubDomains → apply to all subdomains (e.g., admin.sweetshop.com)
            // preload           → eligible for browser HSTS preload lists
            // ⚠️ Only send HSTS over HTTPS – never on HTTP responses.
            if (context.Request.IsHttps)
            {
                h["Strict-Transport-Security"] =
                    "max-age=31536000; includeSubDomains; preload";
            }

            // ── 4. X-XSS-Protection (legacy browser support) ──────────
            // Modern browsers rely on CSP, but this covers older ones.
            h["X-XSS-Protection"] = "1; mode=block";

            // ── 5. Referrer Policy ─────────────────────────────────────
            // Only sends the origin (no path/query) on cross-origin requests,
            // preventing leakage of customer URLs to third-party analytics.
            h["Referrer-Policy"] = "strict-origin-when-cross-origin";

            // ── 6. Permissions Policy ──────────────────────────────────
            // Disables browser features the app doesn't need.
            h["Permissions-Policy"] =
                "camera=(), microphone=(), geolocation=(), payment=()";

            // ── 7. Content Security Policy (CSP) ───────────────────────
            // The most powerful defence against XSS.
            // Adjust the 'self' allow-list to match your actual CDN / font origins.
            h["Content-Security-Policy"] = string.Join("; ",
                // Only load resources from the same origin by default
                "default-src 'self'",

                // Scripts: Allow inline scripts (e.g. razor views) and common CDNs
                "script-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://code.jquery.com https://stackpath.bootstrapcdn.com",

                // Styles: Allow inline styles and common CDNs (Bootstrap, Google Fonts, FontAwesome)
                "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://stackpath.bootstrapcdn.com https://use.fontawesome.com",

                // Fonts: Allow Google Fonts, data URIs, and other CDNs (FontAwesome)
                "font-src 'self' data: https://fonts.gstatic.com https://fonts.googleapis.com https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://use.fontawesome.com https://ka-f.fontawesome.com",

                // Images: Allow local, data URIs (base64 patches), and any HTTPS source
                "img-src 'self' data: https:",

                // AJAX: Allow same origin and common API endpoints if needed (FA Kits often use connect)
                "connect-src 'self' https: https://ka-f.fontawesome.com",

                // Object: Disallow plugins
                "object-src 'none'",

                // Frames: Prevent embedding
                "frame-ancestors 'self'",

                // Forms: Allow submitting to self
                "form-action 'self'",

                // Upgrade insecure requests
                "upgrade-insecure-requests"
            );

            return Task.CompletedTask;
        });

        await next(context);
    }
}
