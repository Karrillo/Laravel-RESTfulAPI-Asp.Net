<?php

namespace App;

use Illuminate\Database\Eloquent\Model;

class Archive extends Model
{
    protected $fillable = [
        'title', 'body', 'user_id', 
    ];

    protected $hidden = [
        'created_at', 'updated_at', 'user_id',
    ];
}
