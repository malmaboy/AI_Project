/// @file
/// @brief This file contains the ::ColorShapeLinks.Common.Session.SessionState
/// enum.
///
/// @author Nuno Fachada
/// @date 2019, 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

namespace ColorShapeLinks.Common.Session
{
    /// <summary>
    /// State of the current session of ColorShapeLinks matches.
    /// </summary>
    public enum SessionState
    {
        /// <summary>Session has just started.</summary>
        Begin,

        /// <summary>A new match is being prepared.</summary>
        PreMatch,

        /// <summary>A match is currently taking place.</summary>
        InMatch,

        /// <summary>A match has just finished.</summary>
        PostMatch,

        /// <summary>The session is being terminated.</summary>
        End
    }
}
