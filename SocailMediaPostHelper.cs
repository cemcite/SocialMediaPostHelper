using System.Collections.Generic;
using System.Linq;

namespace SocialMedia.Post
{
    public class SocailMediaPostHelper
    {
        /// <summary>
        /// Get list of Post
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="post_ids"></param>
        /// <returns></returns>
        List<Post> get_posts(int user_id, List<int> post_ids)
        {
            // Get Posts from Data Layer
            string postQuery = "SELECT * FROM post WHERE id IN post_ids";
            List<PostModel> db_posts = get_data_fromDB<PostModel>(postQuery);

            // Get Users from Data Layer
            List<int> db_posts_user_ids = db_posts.Select(entity => entity.user_id).ToList();
            string userQuery = "SELECT * FROM user WHERE id IN db_posts_user_ids";
            List<UserModel> db_users = get_data_fromDB<UserModel>(userQuery);

            // Get Likes from Data Layer
            string likeQuery = "SELECT post_id FROM like WHERE user_id = user_id AND post_id IN post_ids";
            List<int> db_likedPostIds = get_data_fromDB<int>(likeQuery);

            // Get Following Users from Data Layer
            string followQuery = "SELECT following_id FROM follow WHERE follower_id = user_id";
            List<int> db_followingUserIds = get_data_fromDB<int>(followQuery);

            List<Post> postList = new List<Post>();
            foreach (int post_id in post_ids)
            {
                PostModel db_post = db_posts.SingleOrDefault(post => post.id == post_id);

                // Client Request: Place null values for non-existing posts in the resulting list.
                if (db_post == null)
                {
                    postList.Add(null);
                    continue;
                }

                // Get User data
                UserModel db_user = db_users.Single(user => user.id == db_post.user_id);

                // Generate Post data with details
                postList.Add(new Post
                {
                    id = db_post.id,
                    description = db_post.description,
                    owner = new User
                    {
                        id = db_user.id,
                        username = db_user.username,
                        full_name = db_user.full_name,
                        profile_picture = db_user.profile_picture,
                        followed = db_followingUserIds.Any(followingUserId => followingUserId == db_post.user_id)
                    },
                    image = db_post.image,
                    created_at = db_post.created_at,
                    liked = db_likedPostIds.Any(likedPostId => likedPostId == post_id)
                });
            }

            return postList;
        }

        /// <summary>
        /// Merge in a list the list of posts
        /// </summary>
        /// <param name="list_of_posts"></param>
        /// <returns></returns>
        List<Post> merge_posts(List<List<Post>> list_of_posts)
        {
            List<Post> merge_posts = new List<Post>();
            for (int i = list_of_posts.Count-1; -1 < i ; i--)
            {
                List<Post> posts = list_of_posts[i];
                for(int j = posts.Count-1; -1 < j ; j--)
                {
                    merge_posts.Add(posts[j]);
                }
            }
            return merge_posts;
        }

        /// <summary>
        /// Get Data From DB with SQL query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        List<T> get_data_fromDB<T>(string query)
        {
            /*
             * DB Connection Transactions
             */
            return new List<T>();
        }
    }

    /// <summary>
    /// User definition for UI Layer
    /// </summary>
    class User
    {
        internal int id;
        internal string username;
        internal string full_name;
        internal string profile_picture;
        internal bool followed;   // whether or not requesting user follows
    }
    /// <summary>
    /// Post definition for UI Layer
    /// </summary>
    class Post
    {
        internal int id;
        internal string description;
        internal User owner;
        internal string image;
        internal int created_at;
        internal bool liked;   // whether or not requesting user likes
    }
    /// <summary>
    /// Post definition for Data Layer [DB First]
    /// </summary>
    class PostModel
    {
        internal int id;
        internal string description;
        internal int user_id;
        internal string image;
        internal int created_at;
    }
    /// <summary>
    /// Post definition for Data Layer [DB First]
    /// </summary>
    class UserModel
    {
        internal int id;
        internal string username;
        internal string email;
        internal string full_name;
        internal string profile_picture;
        internal string bio;
        internal string image;
    }
}
